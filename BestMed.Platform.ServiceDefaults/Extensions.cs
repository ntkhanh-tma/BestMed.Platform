using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Text;
using System.Threading.RateLimiting;

namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    /// <summary>Light reads (e.g. GET by ID) — high throughput allowed.</summary>
    public const string RateLimitLight = "light";

    /// <summary>Business operations (e.g. search/query) — moderate throughput.</summary>
    public const string RateLimitStandard = "standard";

    /// <summary>Heavy / sensitive operations (e.g. bulk updates, login) — strict throttling.</summary>
    public const string RateLimitHeavy = "heavy";

    /// <summary>Tag used to evict all cached user responses when user data changes.</summary>
    public const string CacheTagUsers = "users";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Restrict service discovery to HTTPS only
        builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        {
            options.AllowedSchemes = ["https"];
        });

        // Centralized JWT authentication — all services validate tokens issued by BestMed.AuthenticateService
        builder.AddDefaultAuthentication();

        // Rate limiting to prevent abuse
        builder.AddDefaultRateLimiting();

        // Output caching — in-memory by default, swap to Redis when scaling out
        builder.AddDefaultOutputCache();

        return builder;
    }

    /// <summary>
    /// Adds JWT Bearer authentication and an authorization policy that requires authentication by default.
    /// Every endpoint is protected unless explicitly marked with <c>.AllowAnonymous()</c>.
    /// </summary>
    public static TBuilder AddDefaultAuthentication<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException("Jwt:Key is not configured.")))
                };
            });

        builder.Services.AddAuthorizationBuilder()
            // Require authenticated users by default — endpoints must opt out with .AllowAnonymous()
            .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return builder;
    }

    /// <summary>
    /// Adds three tiers of rate limiting, partitioned by client IP:
    /// <list type="bullet">
    ///   <item><c>light</c> — 60 req / 10 s — cheap reads (GET by ID, health probes)</item>
    ///   <item><c>standard</c> — 20 req / 10 s — business queries (search, list, single writes)</item>
    ///   <item><c>heavy</c> — 5 req / 30 s — expensive or sensitive operations (bulk writes, login)</item>
    /// </list>
    /// Apply a policy to an endpoint with <c>.RequireRateLimiting(Extensions.RateLimitLight)</c>.
    /// Endpoints without an explicit policy are not rate-limited beyond normal infrastructure limits.
    /// </summary>
    private static TBuilder AddDefaultRateLimiting<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Light — high-throughput reads
            options.AddFixedWindowLimiter(RateLimitLight, limiter =>
            {
                limiter.PermitLimit = 60;
                limiter.Window = TimeSpan.FromSeconds(10);
                limiter.QueueLimit = 0;
            });

            // Standard — business queries and single-entity writes
            options.AddFixedWindowLimiter(RateLimitStandard, limiter =>
            {
                limiter.PermitLimit = 20;
                limiter.Window = TimeSpan.FromSeconds(10);
                limiter.QueueLimit = 0;
            });

            // Heavy — bulk mutations, login attempts, sensitive operations
            options.AddFixedWindowLimiter(RateLimitHeavy, limiter =>
            {
                limiter.PermitLimit = 5;
                limiter.Window = TimeSpan.FromSeconds(30);
                limiter.QueueLimit = 0;
            });
        });

        return builder;
    }

    /// <summary>
    /// Adds output caching with named policies.
    /// Uses in-memory storage by default. To switch to Redis (for multi-instance deployments):
    /// <list type="number">
    ///   <item>Add <c>Aspire.StackExchange.Redis.OutputCaching</c> package to the service project.</item>
    ///   <item>Call <c>builder.AddRedisOutputCache("redis")</c> in the service's Program.cs <b>after</b> <c>AddServiceDefaults()</c>
    ///         — this replaces the in-memory store with the Aspire-managed Redis instance.</item>
    ///   <item>In the AppHost, add <c>var redis = builder.AddRedis("redis")</c> and <c>.WithReference(redis)</c> to the service.</item>
    /// </list>
    /// </summary>
    private static TBuilder AddDefaultOutputCache<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddOutputCache(options =>
        {
            // "short" — 30 s, suitable for single-entity reads (e.g. GET /users/{id})
            options.AddPolicy("short", policy => policy
                .Expire(TimeSpan.FromSeconds(30))
                .Tag(CacheTagUsers));

            // "query" — 15 s, suitable for list/search endpoints whose results change more often
            options.AddPolicy("query", policy => policy
                .Expire(TimeSpan.FromSeconds(15))
                .SetVaryByQuery("*")
                .Tag(CacheTagUsers));
        });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(tracing =>
                        // Exclude health check requests from tracing
                        tracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Security headers for all responses
        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] = "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()";
            await next(context);
        });

        // Rate limiting middleware
        app.UseRateLimiter();

        // Output caching middleware — must come after auth so cached responses respect authorization
        app.UseOutputCache();

        // All health checks must pass for app to be considered ready to accept traffic after starting
        app.MapHealthChecks(HealthEndpointPath)
            .AllowAnonymous();

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        }).AllowAnonymous();

        return app;
    }
}

using BestMed.AuthenticateService.Services;

namespace BestMed.AuthenticateService;

/// <summary>
/// Extension methods to register AuthenticateService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddAuthServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDataProtection();

        builder.Services.AddHttpClient<IExternalAuthProvider, ExternalAuthProvider>(client =>
        {
            var baseUrl = builder.Configuration["IdentityServer:BaseUrl"]
                ?? throw new InvalidOperationException("IdentityServer:BaseUrl is not configured.");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return builder;
    }
}

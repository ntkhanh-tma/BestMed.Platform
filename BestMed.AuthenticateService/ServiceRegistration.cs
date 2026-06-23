using BestMed.AuthenticateService.Services;

namespace BestMed.AuthenticateService;

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

        builder.Services.AddScoped<IUserBusiness, NullUserBusiness>();
        builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

        builder.Services.AddScoped<LoginResponseBuilder>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ISsoService, SsoService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<ISupportService, SupportService>();

        return builder;
    }
}

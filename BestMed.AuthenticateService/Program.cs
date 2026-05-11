using BestMed.AuthenticateService;
using BestMed.AuthenticateService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDataProtection();

// Register the external auth provider with a named HttpClient pointing to the Identity Server
builder.Services.AddHttpClient<IExternalAuthProvider, ExternalAuthProvider>(client =>
{
    var baseUrl = builder.Configuration["IdentityServer:BaseUrl"]
        ?? throw new InvalidOperationException("IdentityServer:BaseUrl is not configured.");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.AddAuthServiceDefaults();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapAuthEndpoints();

app.Run();

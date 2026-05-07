using BestMed.UserService.Data;
using BestMed.UserService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Database-first: Aspire manages the connection string via the "userdb" resource name.
// For scaffolding, use: dotnet ef dbcontext scaffold "Name=ConnectionStrings:userdb" ...
builder.AddSqlServerDbContext<UserDbContext>("userdb", configureSettings: settings =>
{
    settings.DisableHealthChecks = true;
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapUserEndpoints();

app.Run();

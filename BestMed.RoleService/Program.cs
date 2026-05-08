using BestMed.RoleService;
using BestMed.RoleService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRoleServiceDefaults();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapRoleEndpoints();

app.Run();

using BestMed.ResidentService;
using BestMed.ResidentService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddResidentServiceDefaults();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapResidentEndpoints();

app.Run();

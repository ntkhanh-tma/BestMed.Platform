using BestMed.PrescriberService;
using BestMed.PrescriberService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddPrescriberServiceDefaults();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapPrescriberEndpoints();

app.Run();

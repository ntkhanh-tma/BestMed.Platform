using BestMed.PharmacyService;
using BestMed.PharmacyService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddPharmacyServiceDefaults();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapPharmacyEndpoints();

app.Run();

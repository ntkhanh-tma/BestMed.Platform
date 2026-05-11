using BestMed.WarehouseService;
using BestMed.WarehouseService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddWarehouseServiceDefaults();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapWarehouseEndpoints();

app.Run();

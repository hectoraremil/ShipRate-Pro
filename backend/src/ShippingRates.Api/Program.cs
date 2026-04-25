using ShippingRates.Api.Extensions;
using ShippingRates.Api.Middleware;
using ShippingRates.Application.DependencyInjection;
using ShippingRates.Persistence.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiPipeline();

app.Run();

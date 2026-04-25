using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShippingRates.Application.Interfaces;
using ShippingRates.Persistence.Context;
using ShippingRates.Persistence.Repositories;
using ShippingRatesUnitOfWork = ShippingRates.Persistence.UnitOfWork.UnitOfWork;

namespace ShippingRates.Persistence.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TarifasEnvioDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("La cadena de conexion 'TarifasEnvioDb' no esta configurada.");
        }

        services.AddDbContext<ShippingRatesDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServerOptions =>
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        services.AddScoped<IShippingRateRepository, ShippingRateRepository>();
        services.AddScoped<IShipmentQuoteRepository, ShipmentQuoteRepository>();
        services.AddScoped<IUnitOfWork, ShippingRatesUnitOfWork>();

        return services;
    }
}

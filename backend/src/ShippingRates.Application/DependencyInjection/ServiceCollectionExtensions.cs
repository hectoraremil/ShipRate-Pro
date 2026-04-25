using Microsoft.Extensions.DependencyInjection;
using ShippingRates.Application.Interfaces;
using ShippingRates.Application.Services;
using ShippingRates.Domain.Services;

namespace ShippingRates.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TariffCalculator>();
        services.AddScoped<IShippingRateService, ShippingRateService>();

        return services;
    }
}

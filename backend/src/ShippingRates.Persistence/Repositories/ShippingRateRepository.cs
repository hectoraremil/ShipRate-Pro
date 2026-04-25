using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShippingRates.Application.Interfaces;
using ShippingRates.Domain.Entities;
using ShippingRates.Persistence.Context;

namespace ShippingRates.Persistence.Repositories;

public sealed class ShippingRateRepository : BaseRepository<ShippingRate>, IShippingRateRepository
{
    public ShippingRateRepository(
        ShippingRatesDbContext dbContext,
        ILogger<ShippingRateRepository> logger)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyCollection<Country>> GetAvailableCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteCollectionAsync<Country>(async token => await DbContext.Countries
            .AsNoTracking()
            .Where(country => country.IsActive)
            .OrderBy(country => country.CountryName)
            .ToListAsync(token), "Obtener paises disponibles", cancellationToken);
    }

    public async Task<ShippingRate?> GetActiveRateByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await ExecuteSingleAsync(async token => await QueryAsNoTracking()
            .Include(rate => rate.Country)
            .Where(rate => rate.IsActive
                && rate.Country.IsActive
                && rate.Country.CountryCode == countryCode
                && rate.EffectiveFrom <= utcNow
                && (rate.EffectiveTo == null || rate.EffectiveTo >= utcNow))
            .OrderByDescending(rate => rate.EffectiveFrom)
            .FirstOrDefaultAsync(token), "Obtener tarifa activa por pais", cancellationToken);
    }
}

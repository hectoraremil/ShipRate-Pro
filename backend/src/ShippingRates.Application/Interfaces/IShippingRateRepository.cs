using ShippingRates.Domain.Entities;

namespace ShippingRates.Application.Interfaces;

public interface IShippingRateRepository
{
    Task<IReadOnlyCollection<Country>> GetAvailableCountriesAsync(CancellationToken cancellationToken = default);
    Task<ShippingRate?> GetActiveRateByCountryCodeAsync(string countryCode, CancellationToken cancellationToken = default);
}

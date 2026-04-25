using ShippingRates.Application.Base;
using ShippingRates.Application.DTOs;

namespace ShippingRates.Application.Interfaces;

public interface IShippingRateService
{
    Task<ApplicationResult<IReadOnlyCollection<CountryResponse>>> GetAvailableCountriesAsync(CancellationToken cancellationToken = default);
    Task<ApplicationResult<CalculateQuoteResponse>> CalculateQuoteAsync(
        CalculateQuoteRequest request,
        string? clientIp,
        string? userAgent,
        CancellationToken cancellationToken = default);
    Task<ApplicationResult<CalculateQuoteResponse>> GetQuoteByIdAsync(long shipmentQuoteId, CancellationToken cancellationToken = default);
}

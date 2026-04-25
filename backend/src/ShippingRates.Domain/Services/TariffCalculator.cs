using ShippingRates.Domain.Entities;
using ShippingRates.Domain.Exceptions;

namespace ShippingRates.Domain.Services;

public sealed class TariffCalculator
{
    public ShipmentQuote Calculate(
        decimal weightKg,
        ShippingRate shippingRate,
        string? clientIp,
        string? userAgent)
    {
        if (weightKg <= 0)
        {
            throw new DomainValidationException("Debe ingresar un peso valido mayor que cero.");
        }

        if (shippingRate.RatePerKilogram <= 0)
        {
            throw new DomainValidationException("La tarifa configurada para el pais seleccionado no es valida.");
        }

        return new ShipmentQuote
        {
            CountryId = shippingRate.CountryId,
            RequestedWeightKg = decimal.Round(weightKg, 2, MidpointRounding.AwayFromZero),
            AppliedRatePerKilogram = decimal.Round(shippingRate.RatePerKilogram, 2, MidpointRounding.AwayFromZero),
            TotalAmount = decimal.Round(weightKg * shippingRate.RatePerKilogram, 2, MidpointRounding.AwayFromZero),
            CurrencyCode = shippingRate.CurrencyCode,
            RequestedAt = DateTime.UtcNow,
            ClientIp = clientIp,
            UserAgent = userAgent
        };
    }
}

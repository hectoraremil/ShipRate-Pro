using ShippingRates.Domain.Entities;

namespace ShippingRates.Application.Interfaces;

public interface IShipmentQuoteRepository
{
    Task AddAsync(ShipmentQuote shipmentQuote, CancellationToken cancellationToken = default);
    Task<ShipmentQuote?> GetByIdAsync(long shipmentQuoteId, CancellationToken cancellationToken = default);
}

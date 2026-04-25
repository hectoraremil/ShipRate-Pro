using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShippingRates.Application.Interfaces;
using ShippingRates.Domain.Entities;
using ShippingRates.Persistence.Context;

namespace ShippingRates.Persistence.Repositories;

public sealed class ShipmentQuoteRepository : BaseRepository<ShipmentQuote>, IShipmentQuoteRepository
{
    public ShipmentQuoteRepository(
        ShippingRatesDbContext dbContext,
        ILogger<ShipmentQuoteRepository> logger)
        : base(dbContext, logger)
    {
    }

    public async Task AddAsync(ShipmentQuote shipmentQuote, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shipmentQuote);

        await AddEntityAsync(shipmentQuote, "Registrar cotizacion de envio", cancellationToken);
    }

    public async Task<ShipmentQuote?> GetByIdAsync(long shipmentQuoteId, CancellationToken cancellationToken = default)
    {
        return await ExecuteSingleAsync(async token => await QueryAsNoTracking()
            .Include(quote => quote.Country)
            .FirstOrDefaultAsync(quote => quote.ShipmentQuoteId == shipmentQuoteId, token),
            "Obtener cotizacion por id",
            cancellationToken);
    }
}

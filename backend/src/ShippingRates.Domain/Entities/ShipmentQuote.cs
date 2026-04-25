namespace ShippingRates.Domain.Entities;

public sealed class ShipmentQuote
{
    public long ShipmentQuoteId { get; set; }
    public int CountryId { get; set; }
    public decimal RequestedWeightKg { get; set; }
    public decimal AppliedRatePerKilogram { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime RequestedAt { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }

    public Country Country { get; set; } = null!;
}

namespace ShippingRates.Application.DTOs;

public sealed class CalculateQuoteResponse
{
    public long ShipmentQuoteId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public decimal RatePerKilogram { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime RequestedAt { get; set; }
}

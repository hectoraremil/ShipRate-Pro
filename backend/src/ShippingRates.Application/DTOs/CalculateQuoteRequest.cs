namespace ShippingRates.Application.DTOs;

public sealed class CalculateQuoteRequest
{
    public decimal WeightKg { get; set; }
    public string CountryCode { get; set; } = string.Empty;
}

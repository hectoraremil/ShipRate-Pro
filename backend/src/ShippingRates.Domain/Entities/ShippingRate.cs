namespace ShippingRates.Domain.Entities;

public sealed class ShippingRate
{
    public int ShippingRateId { get; set; }
    public int CountryId { get; set; }
    public decimal RatePerKilogram { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Country Country { get; set; } = null!;
}

namespace ShippingRates.Domain.Entities;

public sealed class Country
{
    public int CountryId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ShippingRate> ShippingRates { get; set; } = new List<ShippingRate>();
}

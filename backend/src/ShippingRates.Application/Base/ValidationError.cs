namespace ShippingRates.Application.Base;

public sealed class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

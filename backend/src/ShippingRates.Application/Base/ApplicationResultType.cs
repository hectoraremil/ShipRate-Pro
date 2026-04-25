namespace ShippingRates.Application.Base;

public enum ApplicationResultType
{
    Success = 0,
    ValidationError = 1,
    NotFound = 2,
    UnexpectedError = 3
}

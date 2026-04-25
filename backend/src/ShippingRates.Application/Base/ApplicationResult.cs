namespace ShippingRates.Application.Base;

public class ApplicationResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public ApplicationResultType ResultType { get; set; }
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; set; } = [];

    public static ApplicationResult<T> Ok(T data, string message)
    {
        return new ApplicationResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            ResultType = ApplicationResultType.Success
        };
    }

    public static ApplicationResult<T> Fail(
        string message,
        ApplicationResultType resultType,
        IReadOnlyCollection<ValidationError>? validationErrors = null)
    {
        return new ApplicationResult<T>
        {
            Success = false,
            Message = message,
            ResultType = resultType,
            ValidationErrors = validationErrors ?? []
        };
    }
}

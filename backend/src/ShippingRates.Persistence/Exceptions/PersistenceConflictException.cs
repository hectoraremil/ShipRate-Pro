namespace ShippingRates.Persistence.Exceptions;

public sealed class PersistenceConflictException : PersistenceException
{
    public PersistenceConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

namespace ShippingRates.Persistence.Exceptions;

public sealed class TransientPersistenceException : PersistenceException
{
    public TransientPersistenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

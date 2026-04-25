using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShippingRates.Persistence.Context;
using ShippingRates.Persistence.Exceptions;

namespace ShippingRates.Persistence.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : class
{
    private static readonly HashSet<int> TransientSqlErrorNumbers =
    [
        -2,
        1205,
        4060,
        40197,
        40501,
        40613,
        49918,
        49919,
        49920
    ];

    protected BaseRepository(ShippingRatesDbContext dbContext, ILogger logger)
    {
        DbContext = dbContext;
        Logger = logger;
        DbSet = dbContext.Set<TEntity>();
    }

    protected ShippingRatesDbContext DbContext { get; }

    protected DbSet<TEntity> DbSet { get; }

    protected ILogger Logger { get; }

    protected IQueryable<TEntity> QueryAsNoTracking()
    {
        return DbSet.AsNoTracking();
    }

    protected IQueryable<TEntity> QueryTracked()
    {
        return DbSet;
    }

    protected async Task<IReadOnlyCollection<TResult>> ExecuteCollectionAsync<TResult>(
        Func<CancellationToken, Task<IReadOnlyCollection<TResult>>> operation,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await operation(cancellationToken);
        }
        catch (Exception exception)
        {
            throw TranslateException(exception, operationName);
        }
    }

    protected async Task<TResult?> ExecuteSingleAsync<TResult>(
        Func<CancellationToken, Task<TResult?>> operation,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await operation(cancellationToken);
        }
        catch (Exception exception)
        {
            throw TranslateException(exception, operationName);
        }
    }

    protected async Task AddEntityAsync(TEntity entity, string operationName, CancellationToken cancellationToken = default)
    {
        try
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            throw TranslateException(exception, operationName);
        }
    }

    protected PersistenceException TranslateException(Exception exception, string operationName)
    {
        Logger.LogError(exception, "Error de persistencia durante la operacion {OperationName}.", operationName);

        if (exception is PersistenceException persistenceException)
        {
            return persistenceException;
        }

        if (exception is DbUpdateConcurrencyException)
        {
            return new PersistenceConflictException(
                "Se detecto un conflicto de concurrencia al acceder a los datos.",
                exception);
        }

        if (exception is DbUpdateException dbUpdateException && IsConflict(dbUpdateException))
        {
            return new PersistenceConflictException(
                "Se produjo un conflicto al guardar los datos en la base de datos.",
                dbUpdateException);
        }

        if (exception is TimeoutException || IsTransient(exception))
        {
            return new TransientPersistenceException(
                "La base de datos no respondio a tiempo. Intente nuevamente.",
                exception);
        }

        return new PersistenceException(
            "Se produjo un error al acceder a la base de datos.",
            exception);
    }

    private static bool IsConflict(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627 || sqlException.Number == 547);
    }

    private static bool IsTransient(Exception exception)
    {
        if (exception is SqlException sqlException)
        {
            return TransientSqlErrorNumbers.Contains(sqlException.Number);
        }

        return exception.InnerException is not null && IsTransient(exception.InnerException);
    }
}

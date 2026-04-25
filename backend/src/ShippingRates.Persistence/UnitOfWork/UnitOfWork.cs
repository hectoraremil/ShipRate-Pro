using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShippingRates.Application.Interfaces;
using ShippingRates.Persistence.Context;
using ShippingRates.Persistence.Exceptions;

namespace ShippingRates.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
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

    private readonly ShippingRatesDbContext dbContext;
    private readonly ILogger<UnitOfWork> logger;

    public UnitOfWork(ShippingRatesDbContext dbContext, ILogger<UnitOfWork> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            throw TranslateException(exception, "Confirmar cambios de persistencia");
        }
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async token =>
        {
            await operation(token);
            return true;
        }, cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        try
        {
            return await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var result = await operation(cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
        catch (Exception exception)
        {
            throw TranslateException(exception, "Ejecutar transaccion de persistencia");
        }
    }

    private PersistenceException TranslateException(Exception exception, string operationName)
    {
        logger.LogError(exception, "Error de persistencia durante la operacion {OperationName}.", operationName);

        if (exception is PersistenceException persistenceException)
        {
            return persistenceException;
        }

        if (exception is DbUpdateConcurrencyException)
        {
            return new PersistenceConflictException(
                "Se detecto un conflicto de concurrencia al guardar los datos.",
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
            "Se produjo un error al confirmar los cambios en la base de datos.",
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

using CommunicationsApp.Infrastructure;

namespace CommunicationsApp.Application.Common;

public abstract class BaseCommandHandler
{
    protected static readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);
    protected IWorkUnit _workUnit;

    public BaseCommandHandler(IWorkUnit workUnit) => _workUnit = workUnit;

    protected async Task<T> WrapInTransactionAsync<T>(Func<Task<T>> asyncFunc)
    {
        await _asyncLock.WaitAsync();

        using var transaction = await _workUnit.BeginTransactionAsync();
        T result;

        try
        {
            result = await asyncFunc();
            await transaction.RollbackAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _asyncLock.Release();
        }

        return result;
    }
}

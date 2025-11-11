using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Infrastructure.Data;


namespace ScaleTrackAPI.Infrastructure.Services.Base   
{
    /// <summary>
    /// Base class for services that require transaction support.
    /// Inherit this class to wrap multiple repository operations in a single transaction.
    /// </summary>
    public abstract class TransactionalServiceBase
    {
        protected readonly AppDbContext _context;

        protected TransactionalServiceBase(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Executes a transactional operation that does not return a value.
        /// </summary>
        protected async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation.Invoke();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Executes a transactional operation that returns a value.
        /// </summary>
        protected async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation.Invoke();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

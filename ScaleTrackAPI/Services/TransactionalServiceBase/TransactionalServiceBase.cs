using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services
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

        /// <summary>
        /// Records an audit trail for any entity changes.
        /// </summary>
        /// <param name="entityName">Entity/table name</param>
        /// <param name="entityId">ID of the affected entity</param>
        /// <param name="action">Action type: Created, Updated, Deleted</param>
        /// <param name="changedBy">User ID performing the action</param>
        /// <param name="original">Original entity before change (optional)</param>
        /// <param name="updated">Updated entity (optional)</param>
        protected async Task RecordAuditAsync(
        string entityName,
        int entityId,
        string action,
        int changedBy,
        object? original = null,
        object? updated = null)
        {
            string changesJson = string.Empty;

            if (original != null || updated != null)
            {
                var changeObj = new
                {
                    Original = original,
                    Updated = updated
                };

                changesJson = JsonSerializer.Serialize(changeObj, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

            }

            var audit = new AuditTrail
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                ChangedBy = changedBy,
                Changes = changesJson,
                ChangedAt = DateTime.UtcNow
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}

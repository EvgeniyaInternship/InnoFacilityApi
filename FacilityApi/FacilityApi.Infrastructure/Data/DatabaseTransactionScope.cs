using FacilityApi.Domain.Interfaces;
using Npgsql;
using System.Data;

namespace FacilityApi.Infrastructure.Data;

internal sealed class DatabaseTransactionScope(NpgsqlConnectionFactory connectionFactory) : IDbTransactionScope
{
    public async Task<IDatabaseTransaction> BeginAsync(CancellationToken cancellationToken = default)
    {
        if (DatabaseTransactionContext.Current is not null)
            throw new InvalidOperationException("A database transaction is already active.");

        var connection = connectionFactory.CreateConnection();
        try
        {
            var transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            DatabaseTransactionContext.Current = transaction;
            return new DatabaseTransaction(connection, transaction);
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    private sealed class DatabaseTransaction(NpgsqlConnection connection, NpgsqlTransaction transaction) : IDatabaseTransaction
    {
        private bool _isCompleted;

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await transaction.CommitAsync(cancellationToken);
            _isCompleted = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_isCompleted)
                return;

            await transaction.RollbackAsync(cancellationToken);
            _isCompleted = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_isCompleted)
                await RollbackAsync();

            if (DatabaseTransactionContext.Current == transaction)
                DatabaseTransactionContext.Current = null;

            await transaction.DisposeAsync();
            await connection.DisposeAsync();
        }
    }
}

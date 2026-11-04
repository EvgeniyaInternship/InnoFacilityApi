using Npgsql;

namespace FacilityApi.Infrastructure.Data;

internal static class DbSession
{
    public static async Task<T> ExecuteAsync<T>(
        NpgsqlConnectionFactory factory,
        Func<NpgsqlConnection, NpgsqlTransaction?, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken)
    {
        var transaction = DatabaseTransactionContext.Current;
        if (transaction?.Connection is NpgsqlConnection connection)
            return await action(connection, transaction, cancellationToken);

        await using var owned = factory.CreateConnection();
        return await action(owned, null, cancellationToken);
    }

    public static Task ExecuteAsync(
        NpgsqlConnectionFactory factory,
        Func<NpgsqlConnection, NpgsqlTransaction?, CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(factory, async (connection, transaction, token) =>
        {
            await action(connection, transaction, token);
            return 0;
        }, cancellationToken);
    }
}

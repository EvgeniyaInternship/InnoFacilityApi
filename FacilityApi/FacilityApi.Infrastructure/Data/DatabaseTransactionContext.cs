using Npgsql;

namespace FacilityApi.Infrastructure.Data;

internal static class DatabaseTransactionContext
{
    private static readonly AsyncLocal<NpgsqlTransaction?> CurrentTransaction = new();

    public static NpgsqlTransaction? Current
    {
        get => CurrentTransaction.Value;
        set => CurrentTransaction.Value = value;
    }
}

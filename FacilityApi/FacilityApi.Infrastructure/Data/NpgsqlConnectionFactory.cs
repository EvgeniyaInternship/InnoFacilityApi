using Microsoft.Extensions.Options;
using Npgsql;

namespace FacilityApi.Infrastructure.Data;

public sealed class NpgsqlConnectionFactory(IOptions<DatabaseOptions> options)
{
    public NpgsqlConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(options.Value.FacilityDb);
        connection.Open();
        return connection;
    }
}

using Dapper;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace FacilityApi.Infrastructure.Repositories;

public sealed class OfficeRepository(NpgsqlConnectionFactory connectionFactory) : IOfficeRepository
{
    public async Task<int> CountAsync(Expression<Func<OfficeEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            $"SELECT COUNT(*) FROM \"Offices\" WHERE {where}",
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<OfficeEntity?> GetAsync(
        Expression<Func<OfficeEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<OfficeEntity>(new CommandDefinition(
            $"""
            SELECT "Id", "Title"
            FROM "Offices"
            WHERE {where}
            LIMIT 1
            """,
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<OfficeEntity>> GetAllAsync(
        Expression<Func<OfficeEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        var sql = SqlPredicateBuilder.AppendPage(
            $"""
            SELECT "Id", "Title"
            FROM "Offices"
            WHERE {where}
            """,
            pageNumber,
            pageSize,
            parameters);
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<OfficeEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(OfficeEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO "Offices" ("Id", "Title")
            VALUES (@Id, @Title)
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task AddRangeAsync(IEnumerable<OfficeEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO "Offices" ("Id", "Title")
            VALUES (@Id, @Title)
            """,
            entities,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(OfficeEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE "Offices"
            SET "Title" = @Title
            WHERE "Id" = @Id
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateRangeAsync(IEnumerable<OfficeEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE "Offices"
            SET "Title" = @Title
            WHERE "Id" = @Id
            """,
            entities,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(OfficeEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM "Offices"
            WHERE "Id" = @Id
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveRangeAsync(IEnumerable<OfficeEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM "Offices"
            WHERE "Id" = @Id
            """,
            entities,
            cancellationToken: cancellationToken));
    }
}

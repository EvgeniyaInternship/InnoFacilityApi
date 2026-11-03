using Dapper;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace FacilityApi.Infrastructure.Repositories;

public sealed class SpecializationRepository(NpgsqlConnectionFactory connectionFactory) : ISpecializationRepository
{
    public async Task<int> CountAsync(Expression<Func<SpecializationEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            $"SELECT COUNT(*) FROM \"Specializations\" WHERE {where}",
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<SpecializationEntity?> GetAsync(
        Expression<Func<SpecializationEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<SpecializationEntity>(new CommandDefinition(
            $"""
            SELECT "Id", "Title", "Description"
            FROM "Specializations"
            WHERE {where}
            LIMIT 1
            """,
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<SpecializationEntity>> GetAllAsync(
        Expression<Func<SpecializationEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        var sql = SqlPredicateBuilder.AppendPage(
            $"""
            SELECT "Id", "Title", "Description"
            FROM "Specializations"
            WHERE {where}
            """,
            pageNumber,
            pageSize,
            parameters);
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<SpecializationEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(SpecializationEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO "Specializations" ("Id", "Title", "Description")
            VALUES (@Id, @Title, @Description)
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task AddRangeAsync(IEnumerable<SpecializationEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO "Specializations" ("Id", "Title", "Description")
            VALUES (@Id, @Title, @Description)
            """,
            entities,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(SpecializationEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE "Specializations"
            SET "Title" = @Title,
                "Description" = @Description
            WHERE "Id" = @Id
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateRangeAsync(IEnumerable<SpecializationEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE "Specializations"
            SET "Title" = @Title,
                "Description" = @Description
            WHERE "Id" = @Id
            """,
            entities,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(SpecializationEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM "Specializations"
            WHERE "Id" = @Id
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveRangeAsync(IEnumerable<SpecializationEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM "Specializations"
            WHERE "Id" = @Id
            """,
            entities,
            cancellationToken: cancellationToken));
    }
}

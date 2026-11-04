using Dapper;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace FacilityApi.Infrastructure.Repositories;

public sealed class ServiceRepository(NpgsqlConnectionFactory connectionFactory) : IServiceRepository
{
    public async Task<int> CountAsync(Expression<Func<ServiceEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            $"SELECT COUNT(*) FROM \"Services\" WHERE {where}",
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<ServiceEntity?> GetAsync(
        Expression<Func<ServiceEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<ServiceEntity>(new CommandDefinition(
            $"""
            SELECT "Id", "Title", "Description", "Price", "IsActive", "DoctorId"
            FROM "Services"
            WHERE {where}
            LIMIT 1
            """,
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<ServiceEntity>> GetAllAsync(
        Expression<Func<ServiceEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        var sql = SqlPredicateBuilder.AppendPage(
            $"""
            SELECT "Id", "Title", "Description", "Price", "IsActive", "DoctorId"
            FROM "Services"
            WHERE {where}
            """,
            pageNumber,
            pageSize,
            parameters);
        using var connection = connectionFactory.CreateConnection();
        return await connection.QueryAsync<ServiceEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(ServiceEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO "Services" ("Id", "Title", "Description", "Price", "IsActive", "DoctorId")
            VALUES (@Id, @Title, @Description, @Price, @IsActive, @DoctorId)
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task AddRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO "Services" ("Id", "Title", "Description", "Price", "IsActive", "DoctorId")
            VALUES (@Id, @Title, @Description, @Price, @IsActive, @DoctorId)
            """,
            entities,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(ServiceEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE "Services"
            SET "Title" = @Title,
                "Description" = @Description,
                "Price" = @Price,
                "IsActive" = @IsActive,
                "DoctorId" = @DoctorId
            WHERE "Id" = @Id
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE "Services"
            SET "Title" = @Title,
                "Description" = @Description,
                "Price" = @Price,
                "IsActive" = @IsActive,
                "DoctorId" = @DoctorId
            WHERE "Id" = @Id
            """,
            entities,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveAsync(ServiceEntity entity, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM "Services"
            WHERE "Id" = @Id
            """,
            entity,
            cancellationToken: cancellationToken));
    }

    public async Task RemoveRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM "Services"
            WHERE "Id" = @Id
            """,
            entities,
            cancellationToken: cancellationToken));
    }
}

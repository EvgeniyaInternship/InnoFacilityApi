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
        return await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteScalarAsync<int>(new CommandDefinition(
                $"SELECT COUNT(*) FROM \"Services\" WHERE {where}",
                parameters,
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task<ServiceEntity?> GetAsync(
        Expression<Func<ServiceEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        return await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.QueryFirstOrDefaultAsync<ServiceEntity>(new CommandDefinition(
                $"""
                SELECT "Id", "Title", "Description", "Price", "IsActive", "DoctorId"
                FROM "Services"
                WHERE {where}
                LIMIT 1
                """,
                parameters,
                transaction,
                cancellationToken: token)),
            cancellationToken);
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
        return await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.QueryAsync<ServiceEntity>(new CommandDefinition(
                sql,
                parameters,
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task AddAsync(ServiceEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteAsync(new CommandDefinition(
                """
                INSERT INTO "Services" ("Id", "Title", "Description", "Price", "IsActive", "DoctorId")
                VALUES (@Id, @Title, @Description, @Price, @IsActive, @DoctorId)
                """,
                entity,
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;

        await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteAsync(new CommandDefinition(
                """
                INSERT INTO "Services" ("Id", "Title", "Description", "Price", "IsActive", "DoctorId")
                VALUES (@Id, @Title, @Description, @Price, @IsActive, @DoctorId)
                """,
                entities,
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task UpdateAsync(ServiceEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteAsync(new CommandDefinition(
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
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;

        await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteAsync(new CommandDefinition(
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
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task RemoveAsync(ServiceEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteAsync(new CommandDefinition(
                """
                DELETE FROM "Services"
                WHERE "Id" = @Id
                """,
                entity,
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }

    public async Task RemoveRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any())
            return;

        await DbSession.ExecuteAsync(
            connectionFactory,
            (connection, transaction, token) => connection.ExecuteAsync(new CommandDefinition(
                """
                DELETE FROM "Services"
                WHERE "Id" = @Id
                """,
                entities,
                transaction,
                cancellationToken: token)),
            cancellationToken);
    }
}

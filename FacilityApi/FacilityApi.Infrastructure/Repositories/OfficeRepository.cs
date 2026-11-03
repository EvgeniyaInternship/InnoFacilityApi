using Dapper;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Constants;
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
            $"SELECT {OfficeRepositoryConstants.Columns} FROM \"Offices\" WHERE {where} LIMIT 1",
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<List<OfficeEntity>> GetAllAsync(
        Expression<Func<OfficeEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        var sql = SqlPredicateBuilder.AppendPage(
            $"SELECT {OfficeRepositoryConstants.Columns} FROM \"Offices\" WHERE {where}",
            pageNumber,
            pageSize,
            parameters);
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<OfficeEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public void Add(OfficeEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(OfficeRepositoryConstants.InsertSql, entity);
    }

    public void AddRange(IEnumerable<OfficeEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(OfficeRepositoryConstants.InsertSql, entities);
    }

    public void Update(OfficeEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(OfficeRepositoryConstants.UpdateSql, entity);
    }

    public void UpdateRange(IEnumerable<OfficeEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(OfficeRepositoryConstants.UpdateSql, entities);
    }

    public void Remove(OfficeEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(OfficeRepositoryConstants.DeleteSql, entity);
    }

    public void RemoveRange(IEnumerable<OfficeEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(OfficeRepositoryConstants.DeleteSql, entities);
    }
}

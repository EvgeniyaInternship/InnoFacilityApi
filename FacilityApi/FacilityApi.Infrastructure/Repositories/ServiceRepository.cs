using Dapper;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Constants;
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
            $"SELECT {ServiceRepositoryConstants.Columns} FROM \"Services\" WHERE {where} LIMIT 1",
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<List<ServiceEntity>> GetAllAsync(
        Expression<Func<ServiceEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        var sql = SqlPredicateBuilder.AppendPage(
            $"SELECT {ServiceRepositoryConstants.Columns} FROM \"Services\" WHERE {where}",
            pageNumber,
            pageSize,
            parameters);
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ServiceEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public void Add(ServiceEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(ServiceRepositoryConstants.InsertSql, entity);
    }

    public void AddRange(IEnumerable<ServiceEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(ServiceRepositoryConstants.InsertSql, entities);
    }

    public void Update(ServiceEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(ServiceRepositoryConstants.UpdateSql, entity);
    }

    public void UpdateRange(IEnumerable<ServiceEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(ServiceRepositoryConstants.UpdateSql, entities);
    }

    public void Remove(ServiceEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(ServiceRepositoryConstants.DeleteSql, entity);
    }

    public void RemoveRange(IEnumerable<ServiceEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(ServiceRepositoryConstants.DeleteSql, entities);
    }
}

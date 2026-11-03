using Dapper;
using FacilityApi.Domain.Entities;
using FacilityApi.Domain.Interfaces;
using FacilityApi.Infrastructure.Constants;
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
            $"SELECT {SpecializationRepositoryConstants.Columns} FROM \"Specializations\" WHERE {where} LIMIT 1",
            parameters,
            cancellationToken: cancellationToken));
    }

    public async Task<List<SpecializationEntity>> GetAllAsync(
        Expression<Func<SpecializationEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var (where, parameters) = SqlPredicateBuilder.Build(predicate);
        var sql = SqlPredicateBuilder.AppendPage(
            $"SELECT {SpecializationRepositoryConstants.Columns} FROM \"Specializations\" WHERE {where}",
            pageNumber,
            pageSize,
            parameters);
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SpecializationEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public void Add(SpecializationEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(SpecializationRepositoryConstants.InsertSql, entity);
    }

    public void AddRange(IEnumerable<SpecializationEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(SpecializationRepositoryConstants.InsertSql, entities);
    }

    public void Update(SpecializationEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(SpecializationRepositoryConstants.UpdateSql, entity);
    }

    public void UpdateRange(IEnumerable<SpecializationEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(SpecializationRepositoryConstants.UpdateSql, entities);
    }

    public void Remove(SpecializationEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(SpecializationRepositoryConstants.DeleteSql, entity);
    }

    public void RemoveRange(IEnumerable<SpecializationEntity> entities)
    {
        if (!entities.Any())
            return;
        using var connection = connectionFactory.CreateConnection();
        connection.Execute(SpecializationRepositoryConstants.DeleteSql, entities);
    }
}

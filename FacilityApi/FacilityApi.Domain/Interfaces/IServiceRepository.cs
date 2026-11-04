using FacilityApi.Domain.Entities;
using System.Linq.Expressions;

namespace FacilityApi.Domain.Interfaces;

public interface IServiceRepository
{
    Task<int> CountAsync(Expression<Func<ServiceEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<ServiceEntity?> GetAsync(Expression<Func<ServiceEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceEntity>> GetAllAsync(
        Expression<Func<ServiceEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default);
    Task AddAsync(ServiceEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(ServiceEntity entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default);
    Task RemoveAsync(ServiceEntity entity, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<ServiceEntity> entities, CancellationToken cancellationToken = default);
}

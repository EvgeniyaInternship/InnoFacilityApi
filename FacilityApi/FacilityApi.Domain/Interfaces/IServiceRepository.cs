using FacilityApi.Domain.Entities;
using System.Linq.Expressions;

namespace FacilityApi.Domain.Interfaces;

public interface IServiceRepository
{
    Task<int> CountAsync(Expression<Func<ServiceEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<ServiceEntity?> GetAsync(Expression<Func<ServiceEntity, bool>> predicate, bool isTracked = false, CancellationToken cancellationToken = default);
    Task<List<ServiceEntity>> GetAllAsync(
        Expression<Func<ServiceEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        bool isTracked = false,
        CancellationToken cancellationToken = default);
    void Add(ServiceEntity entity);
    void AddRange(IEnumerable<ServiceEntity> entities);
    void Update(ServiceEntity entity);
    void UpdateRange(IEnumerable<ServiceEntity> entities);
    void Remove(ServiceEntity entity);
    void RemoveRange(IEnumerable<ServiceEntity> entities);
}

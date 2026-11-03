using FacilityApi.Domain.Entities;
using System.Linq.Expressions;

namespace FacilityApi.Domain.Interfaces;

public interface IOfficeRepository
{
    Task<int> CountAsync(Expression<Func<OfficeEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<OfficeEntity?> GetAsync(Expression<Func<OfficeEntity, bool>> predicate, bool isTracked = false, CancellationToken cancellationToken = default);
    Task<List<OfficeEntity>> GetAllAsync(
        Expression<Func<OfficeEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        bool isTracked = false,
        CancellationToken cancellationToken = default);
    void Add(OfficeEntity entity);
    void AddRange(IEnumerable<OfficeEntity> entities);
    void Update(OfficeEntity entity);
    void UpdateRange(IEnumerable<OfficeEntity> entities);
    void Remove(OfficeEntity entity);
    void RemoveRange(IEnumerable<OfficeEntity> entities);
}

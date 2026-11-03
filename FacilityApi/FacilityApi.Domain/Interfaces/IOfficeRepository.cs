using FacilityApi.Domain.Entities;
using System.Linq.Expressions;

namespace FacilityApi.Domain.Interfaces;

public interface IOfficeRepository
{
    Task<int> CountAsync(Expression<Func<OfficeEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<OfficeEntity?> GetAsync(Expression<Func<OfficeEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<OfficeEntity>> GetAllAsync(
        Expression<Func<OfficeEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default);
    Task AddAsync(OfficeEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<OfficeEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(OfficeEntity entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<OfficeEntity> entities, CancellationToken cancellationToken = default);
    Task RemoveAsync(OfficeEntity entity, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<OfficeEntity> entities, CancellationToken cancellationToken = default);
}

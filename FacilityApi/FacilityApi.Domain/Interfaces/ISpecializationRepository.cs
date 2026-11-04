using FacilityApi.Domain.Entities;
using System.Linq.Expressions;

namespace FacilityApi.Domain.Interfaces;

public interface ISpecializationRepository
{
    Task<int> CountAsync(Expression<Func<SpecializationEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<SpecializationEntity?> GetAsync(Expression<Func<SpecializationEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SpecializationEntity>> GetAllAsync(
        Expression<Func<SpecializationEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        CancellationToken cancellationToken = default);
    Task AddAsync(SpecializationEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<SpecializationEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(SpecializationEntity entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<SpecializationEntity> entities, CancellationToken cancellationToken = default);
    Task RemoveAsync(SpecializationEntity entity, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<SpecializationEntity> entities, CancellationToken cancellationToken = default);
}

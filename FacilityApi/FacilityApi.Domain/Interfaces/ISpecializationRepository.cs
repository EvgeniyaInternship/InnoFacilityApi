using FacilityApi.Domain.Entities;
using System.Linq.Expressions;

namespace FacilityApi.Domain.Interfaces;

public interface ISpecializationRepository
{
    Task<int> CountAsync(Expression<Func<SpecializationEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<SpecializationEntity?> GetAsync(Expression<Func<SpecializationEntity, bool>> predicate, bool isTracked = false, CancellationToken cancellationToken = default);
    Task<List<SpecializationEntity>> GetAllAsync(
        Expression<Func<SpecializationEntity, bool>> predicate,
        int pageNumber = 0,
        int pageSize = 0,
        bool isTracked = false,
        CancellationToken cancellationToken = default);
    void Add(SpecializationEntity entity);
    void AddRange(IEnumerable<SpecializationEntity> entities);
    void Update(SpecializationEntity entity);
    void UpdateRange(IEnumerable<SpecializationEntity> entities);
    void Remove(SpecializationEntity entity);
    void RemoveRange(IEnumerable<SpecializationEntity> entities);
}

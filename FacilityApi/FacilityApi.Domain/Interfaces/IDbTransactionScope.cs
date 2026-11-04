namespace FacilityApi.Domain.Interfaces;

public interface IDbTransactionScope
{
    Task<IDatabaseTransaction> BeginAsync(CancellationToken cancellationToken = default);
}

public interface IDatabaseTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

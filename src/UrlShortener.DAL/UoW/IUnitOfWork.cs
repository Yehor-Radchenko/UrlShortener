namespace UrlShortener.DAL.UoW;

public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveChangesAsync();

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

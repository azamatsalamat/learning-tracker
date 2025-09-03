namespace LearningTracker.Application.Services;

public interface IUnitOfWork {
    Task BeginTransaction(CancellationToken ct);
    Task<bool> CommitAsync(CancellationToken ct);
    Task<bool> RollbackAsync(CancellationToken ct);
    bool TransactionOpened { get; }
    Task FlushChanges(CancellationToken ct);
}
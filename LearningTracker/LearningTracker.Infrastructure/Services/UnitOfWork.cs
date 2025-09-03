using System.Data;
using LearningTracker.Application.Services;
using LearningTracker.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;

namespace LearningTracker.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable {
    private readonly LearningTrackerDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(LearningTrackerDbContext context) {
        _context = context;
    }

    public bool TransactionOpened {
        get { return _transaction != null; }
    }
    
    public async Task BeginTransaction(CancellationToken ct) {
        if (!TransactionOpened) {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }
    }

    public async Task<bool> CommitAsync(CancellationToken ct) {
        if (_transaction == null) {
            return false;
        }

        await _context.SaveChangesAsync(ct);
        await _transaction.CommitAsync(ct);
        _transaction = null;
        return true;
    }

    public async Task<bool> RollbackAsync(CancellationToken ct) {
        if (_transaction == null) {
            return false;
        }

        await _transaction.RollbackAsync(ct);
        _transaction = null;
        return true;
    }

    public Task FlushChanges(CancellationToken ct) {
        return _context.SaveChangesAsync(ct);
    }

    public ValueTask DisposeAsync() {
        return _context.DisposeAsync();
    }

    public void Dispose() {
        _context.Dispose();
    }
}
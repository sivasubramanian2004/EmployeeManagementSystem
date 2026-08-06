using EMS.Data;
using Microsoft.EntityFrameworkCore.Storage;
namespace EMS.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EmsDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(EmsDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
            return;

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
            return;

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
            return;

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}
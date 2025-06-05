using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BeautySpa.Contract.Repositories.IUOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
        Task SaveAsync();
        void BeginTransaction(); 
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default); 
        void CommitTransaction();
        void RollBack();

    }
}
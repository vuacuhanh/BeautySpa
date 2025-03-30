using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core;
using System.Linq.Expressions;
using BeautySpa.Core.Base;
namespace BeautySpa.Contract.Repositories.IUOW
{
    public interface IGenericRepository<T> where T : class
    {
        // query
        IQueryable<T> Entities { get; }

        // non async
        IEnumerable<T> GetAll();
        T? GetById(object id);
        void Insert(T obj);
        void InsertRange(IList<T> obj);
        void Update(T obj);
        void Delete(object id);
        void Save();
        void Delete1(T entity);

        // async
        Task<IList<T>> GetAllAsync();
        Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
        Task<T?> GetByIdAsync(object id);

        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(object id);
        Task SaveAsync();
        Task<T?> GetByKeysAsync(object key1, object key2);
        //thêm
        Task<IList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
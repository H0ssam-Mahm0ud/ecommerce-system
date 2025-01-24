using System.Linq.Expressions;

namespace Ecommerce.Models.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? perdicate = null, string? includedWord = null);
        T GetItem(Expression<Func<T, bool>>? perdicate = null, string? includedWord = null);
        void Add(T item);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items); 
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IServiceRepo<T>
    {
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T[] entity);
        void DeleteById(int id);
        IEnumerable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        void AddAsync(T entity);
        void DeleteByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}

using Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceRepo<TEntity> : IServiceRepo<TEntity> where TEntity : class
    {
        private readonly DbContext _DBContext;
        private readonly DbSet<TEntity> _Entities;
        public ServiceRepo(DbContext dBContext)
        {
            _DBContext = dBContext;
            _Entities = dBContext.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _Entities.AsEnumerable();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _Entities.ToListAsync();
        }

        public TEntity GetById(int id)
        {
            return _Entities.Find(id);
        }
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _Entities.FindAsync(id);
        }

        public void Add(TEntity entity)
        {
            _Entities.Add(entity);
        }
        public async void AddAsync(TEntity entity)
        {
            await _Entities.AddAsync(entity);
        }

        public void DeleteById(int id)
        {
            var entity = GetById(id);
            _Entities.Remove(entity);
        }
        public async void DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            _Entities.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _Entities.Update(entity);
        }
        public void Delete(TEntity[] entity)
        {
            _Entities.RemoveRange(entity);
        }
    }
}

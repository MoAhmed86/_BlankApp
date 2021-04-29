using Core.Services;
using DB;
using System.Threading.Tasks;

namespace Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _DBContext;

        public UnitOfWork(AppDBContext dbContext)
        {
            _DBContext = dbContext;
        }

        public int Complete()
        {
            return _DBContext.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await _DBContext.SaveChangesAsync();
        }
    }
}

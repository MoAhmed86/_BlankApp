using System.Threading.Tasks;

namespace Core.Services
{
    public interface IUnitOfWork
    {
        int Complete();
        Task CompleteAsync();
    }
}

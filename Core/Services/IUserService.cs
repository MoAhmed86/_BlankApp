using Core.DTOs.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IUserService : IServiceRepo<ApplicationUser>
    {
        Task<IList<ApplicationUser>> GetAsync( string claimType);
    }
}
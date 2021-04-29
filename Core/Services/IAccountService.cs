using Core.DTOs;
using Core.DTOs.Identity;
using Core.DTOs.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAccountService
    {
        Task<ResponseDto<ApplicationUser>> SaveUserAsync(RegisterDto model, string role,int userId);

        Task<IEnumerable<ApplicationUser>> GetUsers(string claimType = null);

        Task<PagedListModel<ApplicationUser>> GetUsers(UserSearchModel userSearchModel);

        Task<ResponseDto<ApplicationUser>> GetUser(int id);
        Task<ResponseDto<ApplicationUser>> GetUserByEmail(string email);

        Task<ResponseDto> DeleteUserAsync(int id);

        ResponseDto<IEnumerable<ApplicationRole>> GetRoles();

        Task<PagedListModel<ApplicationRole>> GetRoles(RoleSearchModel roleSearchModel);

        Task<ResponseDto<ApplicationRole>> GetRoleAsync(int id);

        Task<ResponseDto<ApplicationRole>> DeleteRoleAsync(int id, int userId);
    }
}

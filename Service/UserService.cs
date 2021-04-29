using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Core.DTOs.Identity;
using Core.Services;
using Service;

namespace DB
{
    public class UserService : ServiceRepo<ApplicationUser>, IUserService
    {
        private readonly AppDBContext context;

        public UserService(AppDBContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IList<ApplicationUser>> GetAsync(string claimType)
        {
            var query = from u in context.Users
                        join ur in context.UserRoles on u.Id equals ur.UserId
                        join r in context.Roles on ur.RoleId equals r.Id
                        join c in context.RoleClaims on r.Id equals c.RoleId
                        where c.ClaimType == claimType
                        //Exclude users with no email (API Accounts)
                        && !string.IsNullOrEmpty(u.Email)
                        select u;

            return await query.Distinct().ToListAsync();
        }
    }
}
using System.Linq;
using AutoMapper;
using Core;
using Core.DTOs;
using Core.DTOs.Identity;
using Core.Services;
using DB;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Core.DTOs.Search;
using static Core.Enums;
using Core.Resources;

namespace Service
{
    public class AccountService : IAccountService
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AppDBContext dbContext;
        private readonly IUnitOfWork uow;
        private readonly ICommonFunctions commonFunctions;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IOptions<Configuration> config;
        private readonly IUserService userService;

        public AccountService(IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDBContext dbContext, IUnitOfWork uow, ICommonFunctions commonFunctions, RoleManager<ApplicationRole> roleManager, IOptions<Configuration> config
            , IUserService userService)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
            this.uow = uow;
            this.commonFunctions = commonFunctions;
            this.roleManager = roleManager;
            this.config = config;
            this.userService = userService;
        }

        public async Task<ResponseDto<ApplicationUser>> SaveUserAsync(RegisterDto dto, string role, int userId)
        {
            var user = mapper.Map<ApplicationUser>(dto);

            try
            {
                IdentityResult result = null;

                if (dto.Id.HasValue && dto.Id > 0)
                {
                    //Check if IdentityNumber registered before
                    if (dbContext.Users.Any(a => a.IdentityNumber == dto.IdentityNumber && a.Id != dto.Id))
                        return new ResponseDto<ApplicationUser> { Data = user, Message = $"{Resource.IdentityNumber} {Resource.PreviouslyRegistered}", StatusCode = StatusCode.Duplicated };

                    var userEntity = await userManager.Users.SingleOrDefaultAsync(w => w.Id == dto.Id);

                    if (userEntity != null)
                    {
                        mapper.Map(dto, userEntity, typeof(RegisterDto), typeof(ApplicationUser));
                        userEntity.LastUpdatedBy = userId;
                        userEntity.LastUpdateDate = DateTime.Now;
                        result = await userManager.UpdateAsync(userEntity);
                    }
                    else
                        return new ResponseDto<ApplicationUser> { Data = user, Message = "", StatusCode = StatusCode.NotFound };
                }
                else
                {
                    //Check if IdentityNumber registered before
                    if (dbContext.Users.Any(a => a.IdentityNumber == dto.IdentityNumber))
                        return new ResponseDto<ApplicationUser> { Data = user, Message = $"{Resource.IdentityNumber} {Resource.PreviouslyRegistered}", StatusCode = StatusCode.Duplicated };

                    user.CreatedBy = userId;
                    user.CreationDate = DateTime.Now;

                    result = await userManager.CreateAsync(user, dto.Password);

                    if (result.Succeeded && !string.IsNullOrEmpty(role))
                        await userManager.AddToRoleAsync(user, role);
                }

                if (result.Succeeded)
                {
                    if (result.Succeeded)
                        return new ResponseDto<ApplicationUser> { Data = user, Message = "", StatusCode = StatusCode.OK };
                    else
                        await userManager.DeleteAsync(user);
                }
                else if (result.Errors.Where(w => w.Code == "DuplicateUserName").Count() > 0)
                {
                    return new ResponseDto<ApplicationUser> { Data = user, Message = "", StatusCode = StatusCode.Duplicated };
                }

                return new ResponseDto<ApplicationUser> { Data = user, Message = "", StatusCode = StatusCode.SaveFailed };
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, dto);
                return new ResponseDto<ApplicationUser> { Data = user, Message = "", StatusCode = StatusCode.InternalError };
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers(string claimType = null)
        {
            IEnumerable<ApplicationUser> applicationUsers = null;

            if (string.IsNullOrEmpty(claimType))//Exclude users with no email (API accounts)
                applicationUsers = userManager.Users.Where(w => !string.IsNullOrEmpty(w.Email)).ToList();
            else
                applicationUsers = await userService.GetAsync(claimType);
            return applicationUsers;
        }

        public async Task<PagedListModel<ApplicationUser>> GetUsers(UserSearchModel userSearchModel)
        {
            PagedListModel<ApplicationUser> users = new PagedListModel<ApplicationUser>();

            IList<ApplicationUser> applicationUsers = null;
            int count = 0;
            if (string.IsNullOrEmpty(userSearchModel.RoleName))
            {
                count = await userManager.Users
                        .Where(w => (string.IsNullOrEmpty(userSearchModel.FullName) || w.FullName.Contains(userSearchModel.FullName))
                        && (string.IsNullOrEmpty(userSearchModel.Email) || w.Email == userSearchModel.Email)
                        && (!userSearchModel.IsDeleted.HasValue || w.IsDeleted == userSearchModel.IsDeleted || (userSearchModel.IsDeleted == false && w.IsDeleted != true))
                        //Excluding users without Email address (API Accounts)
                        && !string.IsNullOrEmpty(w.Email))
                        .CountAsync();

                applicationUsers = await userManager.Users.Include("ApplicationUserRoles.Role")
                        .Where(w => (string.IsNullOrEmpty(userSearchModel.FullName) || w.FullName.Contains(userSearchModel.FullName))
                        && (string.IsNullOrEmpty(userSearchModel.Email) || w.Email == userSearchModel.Email)
                        && (!userSearchModel.IsDeleted.HasValue || w.IsDeleted == userSearchModel.IsDeleted || (userSearchModel.IsDeleted == false && w.IsDeleted != true))
                        //Excluding users without Email address (API Accounts)
                        && !string.IsNullOrEmpty(w.Email))
                        .OrderByDescending(o => o.CreationDate)
                        .Skip(userSearchModel.PageIndex * config.Value.PageSize).Take(config.Value.PageSize)
                        .ToListAsync();
            }
            else
            {
                var tmpUsers = await userManager.GetUsersInRoleAsync(userSearchModel.RoleName);
                count = tmpUsers
                        .Where(w => (string.IsNullOrEmpty(userSearchModel.FullName) || w.FullName.Contains(userSearchModel.FullName))
                        && (string.IsNullOrEmpty(userSearchModel.Email) || w.Email == userSearchModel.Email)
                        && (!userSearchModel.IsDeleted.HasValue || w.IsDeleted == userSearchModel.IsDeleted
                        || (userSearchModel.IsDeleted == false && w.IsDeleted != true)))
                        .Count();

                applicationUsers = tmpUsers
                        .Where(w => (string.IsNullOrEmpty(userSearchModel.FullName) || w.FullName.Contains(userSearchModel.FullName))
                        && (string.IsNullOrEmpty(userSearchModel.Email) || w.Email == userSearchModel.Email)
                        && (!userSearchModel.IsDeleted.HasValue || w.IsDeleted == userSearchModel.IsDeleted
                        || (userSearchModel.IsDeleted == false && w.IsDeleted != true)))
                        .OrderByDescending(o => o.CreationDate)
                        .Skip(userSearchModel.PageIndex * config.Value.PageSize).Take(config.Value.PageSize).ToList();
            }

            users.PagingModel = commonFunctions.PreparePagingModel(userSearchModel.PageIndex, count);
            users.ListObj = applicationUsers;

            return users;
        }

        public async Task<ResponseDto<ApplicationUser>> GetUser(int id)
        {
            ResponseDto<ApplicationUser> response = new ResponseDto<ApplicationUser>();

            try
            {
                response.Data = await userManager.FindByIdAsync(id.ToString());

                response.StatusCode = Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                response.StatusCode = Enums.StatusCode.InternalError;
            }

            return response;
        }

        public async Task<ResponseDto<ApplicationUser>> GetUserByEmail(string email)
        {
            ResponseDto<ApplicationUser> response = new ResponseDto<ApplicationUser>();

            try
            {
                response.Data = await userManager.FindByEmailAsync(email);

                response.StatusCode = Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, email);
                response.StatusCode = Enums.StatusCode.InternalError;
            }

            return response;
        }

        public async Task<ResponseDto> DeleteUserAsync(int id)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                var userResponse = await GetUser(id);
                if (userResponse.StatusCode == Enums.StatusCode.OK)
                {
                    userResponse.Data.IsDeleted = !userResponse.Data.IsDeleted.HasValue || userResponse.Data.IsDeleted == false ? true : false;
                    var result = await userManager.UpdateAsync(userResponse.Data);

                    if (result.Succeeded)
                        response.StatusCode = userResponse.Data.IsDeleted.HasValue && userResponse.Data.IsDeleted == true ? StatusCode.Deactivated : StatusCode.Activated;
                    else
                        response.StatusCode = Enums.StatusCode.SaveFailed;
                }
                else
                    return userResponse;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                response.StatusCode = Enums.StatusCode.InternalError;
            }

            return response;
        }

        #region Roles

        public async Task<ResponseDto<ApplicationRole>> GetRoleAsync(int id)
        {
            ResponseDto<ApplicationRole> response = new ResponseDto<ApplicationRole>();

            try
            {
                response.Data = await roleManager.FindByIdAsync(id.ToString());
                response.StatusCode = Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                response.StatusCode = Enums.StatusCode.InternalError;
            }

            return response;
        }

        public async Task<ResponseDto<ApplicationRole>> DeleteRoleAsync(int id, int userId)
        {
            ResponseDto<ApplicationRole> response = new ResponseDto<ApplicationRole>();

            try
            {
                IdentityResult result = null;
                if (id > 0)
                {
                    var roleResponse = await GetRoleAsync(id);
                    if (roleResponse.StatusCode == Enums.StatusCode.OK)
                    {
                        roleResponse.Data.IsDeleted = !roleResponse.Data.IsDeleted.HasValue || roleResponse.Data.IsDeleted == false ? true : false;
                        roleResponse.Data.LastUpdatedBy = userId;
                        roleResponse.Data.LastUpdateDate = DateTime.Now;

                        result = await roleManager.UpdateAsync(roleResponse.Data);

                        if (result.Succeeded)
                            response.StatusCode = roleResponse.Data.IsDeleted.HasValue && roleResponse.Data.IsDeleted == true ? StatusCode.Deactivated : StatusCode.Activated;
                        else
                            response.StatusCode = Enums.StatusCode.SaveFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                response.StatusCode = Enums.StatusCode.InternalError;
            }

            return response;
        }

        public ResponseDto<IEnumerable<ApplicationRole>> GetRoles()
        {
            ResponseDto<IEnumerable<ApplicationRole>> response = new ResponseDto<IEnumerable<ApplicationRole>>();

            try
            {
                response.Data = roleManager.Roles.ToList();
                response.StatusCode = Enums.StatusCode.OK;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
                response.StatusCode = Enums.StatusCode.InternalError;
            }

            return response;
        }

        public async Task<PagedListModel<ApplicationRole>> GetRoles(RoleSearchModel roleSearchModel)
        {
            PagedListModel<ApplicationRole> rolesPagedList = new PagedListModel<ApplicationRole>();

            int count = await roleManager.Roles
                .Where(w => (string.IsNullOrEmpty(roleSearchModel.RoleName) || w.Name.Contains(roleSearchModel.RoleName))
                && (!roleSearchModel.IsDeleted.HasValue || w.IsDeleted == roleSearchModel.IsDeleted
                        || (roleSearchModel.IsDeleted == false && w.IsDeleted != true)))
                .CountAsync();

            IList<ApplicationRole> roles = await roleManager.Roles
                .Where(w => (string.IsNullOrEmpty(roleSearchModel.RoleName) || w.Name.Contains(roleSearchModel.RoleName))
                && (!roleSearchModel.IsDeleted.HasValue || w.IsDeleted == roleSearchModel.IsDeleted
                        || (roleSearchModel.IsDeleted == false && w.IsDeleted != true)))
                .OrderByDescending(o => o.CreationDate)
                .Skip(roleSearchModel.PageIndex * config.Value.PageSize).Take(config.Value.PageSize)
                .ToListAsync();

            rolesPagedList.PagingModel = commonFunctions.PreparePagingModel(roleSearchModel.PageIndex, count);
            rolesPagedList.ListObj = roles;

            return rolesPagedList;
        }

        #endregion
    }
}

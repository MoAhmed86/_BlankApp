using System.Linq;
using Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static Core.Enums;
using Microsoft.Extensions.Options;
using Core.DTOs;
using Core.Services;
using DB;

namespace Service
{
    public class LookupItemService : ServiceRepo<LookupItem>, ILookupItemService
    {
        private readonly AppDBContext context;
        private readonly IOptions<Configuration> config;

        public LookupItemService(AppDBContext context, IOptions<Configuration> config) : base(context)
        {
            this.context = context;
            this.config = config;
        }

        public async Task<int> GetMaxIdAsync()
        {
            return await context.LookupItems.MaxAsync(m => m.Id);
        }

        public async Task<IList<LookupItem>> GetAllAsync(int parentId)
        {
            return await context.LookupItems.Where(w => w.ParentId == (int)parentId).ToListAsync();
        }

        public async Task<int> OrganizersCountAsync(string platformName,string organizerName, bool? isDeleted)
        {
            return await context.LookupItems
                .Where(w => w.ParentLookupItem.ParentId == (int)LookupItemCategories.Platforms
                && (!isDeleted.HasValue || w.IsDeleted == isDeleted || (isDeleted == false && w.IsDeleted != true))
                && (string.IsNullOrEmpty(platformName) || w.ParentLookupItem.DescAr.Contains(platformName) || w.ParentLookupItem.DescEn.Contains(platformName))
                && (string.IsNullOrEmpty(organizerName) || w.DescAr.Contains(organizerName) || w.DescEn.Contains(organizerName)))
                .CountAsync();
        }

        public async Task<IList<LookupItem>> GetOrganizersAsync(string platformName, string organizerName, bool? isDeleted, int pageIndex)
        {
            return await context.LookupItems
                .Include(i => i.ParentLookupItem)
                .Where(w => w.ParentLookupItem.ParentId == (int)LookupItemCategories.Platforms
                && (!isDeleted.HasValue || w.IsDeleted == isDeleted || (isDeleted == false && w.IsDeleted != true))
                && (string.IsNullOrEmpty(platformName) || w.ParentLookupItem.DescAr.Contains(platformName) || w.ParentLookupItem.DescEn.Contains(platformName))
                && (string.IsNullOrEmpty(organizerName) || w.DescAr.Contains(organizerName) || w.DescEn.Contains(organizerName)))
                .OrderByDescending(o => o.CreationDate)
                .Skip(pageIndex * config.Value.PageSize).Take(config.Value.PageSize)
                .ToListAsync();
        }


        //public async Task<IList<LookupItemDto>> GetAllAsync(int parentId)
        //{
        //    var entities = await lookupItemRepository.GetAllAsync(parentId);
        //    return mapper.Map<IList<LookupItemDto>>(entities);
        //}

        //public async Task<LookupItemDto> GetAsync(int id)
        //{
        //    var entities = await lookupItemRepository.GetByIdAsync(id);
        //    return mapper.Map<LookupItemDto>(entities);
        //}

        //public async Task<ResponseDto> SaveAsync(LookupItemDto dto)
        //{
        //    ResponseDto response = new ResponseDto();

        //    if (dto.Id > 0)
        //    {
        //        //Edit
        //        var entity = await lookupItemRepository.GetByIdAsync(dto.Id);
        //        if (entity != null)
        //        {
        //            mapper.Map(dto, entity, typeof(LookupItemDto), typeof(LookupItem));
        //            lookupItemRepository.Update(entity);

        //            await uow.CompleteAsync();
        //            response.StatusCode = StatusCode.OK;
        //        }
        //        else
        //            response.StatusCode = StatusCode.NotFound;
        //    }
        //    else
        //    {
        //        //New
        //        var lookupItem = mapper.Map<LookupItem>(dto);

        //        //Getting max Id
        //        int maxId = await lookupItemRepository.GetMaxIdAsync();
        //        lookupItem.Id = maxId >= 10000 ? maxId + 1 : 10000;
        //        lookupItemRepository.AddAsync(lookupItem);

        //        await uow.CompleteAsync();
        //        response.StatusCode = StatusCode.OK;
        //    }

        //    return response;
        //}

        //public async Task<ResponseDto<PagedListModel<OrganizerDto>>> OrganizerList(string platformName, string organizerName, bool? isDeleted, int pageIndex)
        //{
        //    ResponseDto<PagedListModel<OrganizerDto>> response = new ResponseDto<PagedListModel<OrganizerDto>>();

        //    try
        //    {
        //        if (response.Data == null)
        //            response.Data = new PagedListModel<OrganizerDto>();

        //        int count = await lookupItemRepository.OrganizersCountAsync(platformName, organizerName, isDeleted);
        //        IList<LookupItem> entityList = await lookupItemRepository.GetOrganizersAsync(platformName, organizerName, isDeleted, pageIndex);

        //        response.Data.PagingModel = commonFunctions.PreparePagingModel(pageIndex, count);
        //        response.Data.ListObj = mapper.Map<IList<OrganizerDto>>(entityList);
        //        response.StatusCode = StatusCode.OK;
        //    }
        //    catch (Exception ex)
        //    {
        //        commonFunctions.LogError(ex);
        //        response.StatusCode = StatusCode.InternalError;
        //    }

        //    return response;
        //}

        //public async Task<ResponseDto> DeleteOrganizerAsync(int id)
        //{
        //    ResponseDto response = new ResponseDto();

        //    try
        //    {
        //        var entity = await lookupItemRepository.GetByIdAsync(id);
        //        entity.IsDeleted = !entity.IsDeleted.HasValue || entity.IsDeleted == false ? true : false;
        //        lookupItemRepository.Update(entity);
        //        await uow.CompleteAsync();
        //        response.StatusCode = entity.IsDeleted.HasValue && entity.IsDeleted == true ? StatusCode.Deactivated : StatusCode.Activated;
        //    }
        //    catch (Exception ex)
        //    {
        //        commonFunctions.LogError(ex, id);
        //        response.StatusCode = StatusCode.InternalError;
        //    }

        //    return response;
        //}
    }
}
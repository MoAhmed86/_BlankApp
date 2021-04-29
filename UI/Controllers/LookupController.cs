using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Core.Enums;

namespace UI.Controllers
{
    public class LookupController : Controller
    {
        private readonly ILookupItemService lookupItemService;
        private readonly ICommonFunctions commonFunctions;

        public LookupController(ILookupItemService lookupItemService,ICommonFunctions commonFunctions)
        {
            this.lookupItemService = lookupItemService;
            this.commonFunctions = commonFunctions;
        }

        [Authorize(Policy = "ManageOrganizers")]
        public IActionResult OrganizersList()
        {
            IList<LookupItemDto> lkpItems = new List<LookupItemDto>();
            lkpItems.Add(new LookupItemDto()
            {
                Id = 0,
                DescAr = Resource_Core.Active,
            });
            lkpItems.Add(new LookupItemDto()
            {
                Id = 1,
                DescAr = Resource_Core.InActive,
            });

            ViewBag.Status = lkpItems;
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ManageOrganizers")]
        public async Task<PartialViewResult> OrganizersListPartial(string platformName,string organizerName, bool? isDeleted,int pageIndex)
        {
            ResponseDto<PagedListModel<OrganizerDto>> response = new ResponseDto<PagedListModel<OrganizerDto>>();

            try
            {
                response = await lookupItemService.OrganizerList(platformName, organizerName,isDeleted, pageIndex);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, new {  platformName , organizerName , pageIndex});
            }

            return PartialView("_OrganizersList", response.Data);
        }

        [Authorize(Policy = "ManageOrganizers")]
        public async Task<IActionResult> Organizer(int? id)
        {
            LookupItemDto dto = new LookupItemDto();

            if (id.HasValue && id > 0)
                dto = await lookupItemService.GetAsync(id.GetValueOrDefault());

            ViewBag.Platforms = await lookupItemService.GetAllAsync((int)LookupItemCategories.Platforms);
            return View(dto);
        }

        [HttpPost]
        [Authorize(Policy = "ManageOrganizers")]
        public async Task<IActionResult> SaveOrganizer(LookupItemDto dto)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                if (ModelState.IsValid)
                {
                    response = await lookupItemService.SaveAsync(dto);
                    if (response.StatusCode == Core.Enums.StatusCode.OK)
                    {
                        TempData["Success"] = Resource_Core.SaveOrganizerSucceeded;
                        return RedirectToAction("OrganizersList", "Lookup");
                    }
                    else
                        TempData["Error"] = Resource_Core.InternalErrorWhileLoading;
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, dto);
                response.StatusCode = Core.Enums.StatusCode.InternalError;
                TempData["Error"] = Resource_Core.InternalErrorWhileLoading;
            }

            ViewBag.Platforms = await lookupItemService.GetAllAsync((int)LookupItemCategories.Platforms);
            return View("Organizer", dto);
        }

        [HttpPost]
        [Authorize(Policy = "ManageOrganizers")]
        public async Task<IActionResult> DeleteOrganizer(int id)
        {
            try
            {
                var response = await lookupItemService.DeleteOrganizerAsync(id);
                if (response.StatusCode == Core.Enums.StatusCode.Activated)
                    TempData["Success"] = Resource_Core.ActivatingOrganizerSucceeded;
                else if (response.StatusCode == Core.Enums.StatusCode.Deactivated)
                    TempData["Success"] = Resource_Core.DeleteOrganizerSucceeded;
                else
                    return RedirectToAction("E500", "Error");
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                return RedirectToAction("E500", "Error");
            }

            return RedirectToAction("OrganizersList");
        }

    }
}
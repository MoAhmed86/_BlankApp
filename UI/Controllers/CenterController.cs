using System;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class CenterController : Controller
    {
        private readonly ICenterService centerService;

        public CenterController(ICenterService centerService)
        {
            this.centerService = centerService;
        }

        public async Task<IActionResult> Index(int id)
        {
            try
            {
                ResponseDto<CenterDto> responseDto = await centerService.GetAsync(id);
                if (responseDto.StatusCode == Core.Enums.StatusCode.OK)
                    return View(responseDto.Data);
                else if (responseDto.StatusCode == Core.Enums.StatusCode.NotFound)
                    return RedirectToAction("E404", "Error");
                else
                    return RedirectToAction("E500", "Error");
            }
            catch
            {
                return RedirectToAction("E500", "Error");
            }
        }

        public async Task<IActionResult> Create(int? id)
        {
            ResponseDto<CenterDto> responseDto = new ResponseDto<CenterDto>();

            if (id.HasValue && id > 0)
            {
                responseDto = await centerService.GetAsync(id.Value);
                if (responseDto.StatusCode == Core.Enums.StatusCode.NotFound)
                    return RedirectToAction("E404", "Error");
            }

            return View(responseDto.Data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(CenterDto dto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await centerService.SaveAsync(dto);
                    if (response.StatusCode == Core.Enums.StatusCode.OK || response.StatusCode == Core.Enums.StatusCode.Updated)
                    {
                        TempData["Success"] = Resource.SavedSuccessfully;
                        return RedirectToAction("Index", new { id = response.Data });
                    }
                    else
                        TempData["Error"] = Resource.SaveFailed;
                }

                return View(dto);
            }
            catch
            {
                return RedirectToAction("E500", "Error");
            }
        }
    }
}

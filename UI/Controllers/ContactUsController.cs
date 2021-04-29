using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;
using Core.DTOs.Identity;
using Core.DTOs.ServiceNow;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using UI.Validators;
using Core.Resources;
using System.Collections.Generic;
using System;
using AutoMapper;

namespace UI.Controllers
{
    public class ContactUsController : Controller
    {
        // GET: ContactUs
        private readonly IServiceNowService _serviceNowService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOptions<Configuration> config;
        private readonly IMapper mapper;
        private readonly ICommonFunctions commonFunctions;

        public ContactUsController(IServiceNowService serviceNowService, UserManager<ApplicationUser> userManager, IOptions<Configuration> config, IMapper mapper
            , ICommonFunctions commonFunctions)
        {
            _serviceNowService = serviceNowService;
            this.userManager = userManager;
            this.config = config;
            this.mapper = mapper;
            this.commonFunctions = commonFunctions;
        }

        public ActionResult Index()
        {
            // get reCAPTHCA key from appsettings.json
            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;

            return View(PrepareContactUsModel());
        }

        private ContactUsVM PrepareContactUsModel()
        {
            try
            {
                ApplicationUser userModel = userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
                string identityNo = string.Empty;
                if (userModel != null)
                    identityNo = userModel.IdentityNumber;

                var model = new ContactUsVM()
                {
                    TicketVM = new TicketVM { IdentityNumber = identityNo, Source = config.Value.ServiceNowSourceName, IsLoggedIn = User.Identity.IsAuthenticated }
                };

                return model;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
            }

            return null;
        }

        //    [HttpPost]
        //    [AllowAnonymous]
        //    public async Task<JsonResult> Send(ContactUsVM model)
        //    {
        //        ResponseModel responseModel = new ResponseModel();
        //        ContactUsValidator validator = new ContactUsValidator();
        //        var validatorResult = validator.Validate(model);
        //        if (validatorResult.IsValid)
        //        {
        //            responseModel.StatusCode = await _contactUSServic.Create(model);

        //        }
        //        else
        //            PrepareInvalidModeErrors(responseModel, validatorResult);

        //        return Json(responseModel);
        //    }

        private void PrepareInvalidModeErrors(ResponseDto responseModel, FluentValidation.Results.ValidationResult validatorResult)
        {
            responseModel.StatusCode = Core.Enums.StatusCode.InvalidModel;
            responseModel.ValidationErrors = validatorResult.Errors.Select(s => new ValidationErrorModel() { ErrorMessage = s.ErrorMessage, PropertyName = s.PropertyName }).ToList();

            //if ( Request.Form["g-recaptcha-response"] != null && string.IsNullOrEmpty(Request.Form["g-recaptcha-response"]))
            //    responseModel.ValidationErrors.Add(new ValidationErrorModel()
            //    {
            //        ErrorMessage = Resource.ReCaptcha_Required,
            //        PropertyName = "g-recaptcha-response",
            //    });
        }

        //    [AuthorizeAdmin]
        //    public ActionResult AdminReview()
        //    {
        //        return View();
        //    }

        //    [AuthorizeAdmin]
        //    public async Task<PartialViewResult> Reviews(int pageIndex)
        //    {
        //        try
        //        {
        //            var responseDto = await _contactUSServic.GetListContactUsVM(pageIndex);

        //            return PartialView("_Reviews", responseDto.Data);
        //        }
        //        catch (Exception ex)
        //        {
        //            CommonFunctions.LogError(ex);
        //            return PartialView("E500");
        //        }
        //    }

        //    [AuthorizeAdmin]
        //    public async Task<ActionResult> GetDetails(int id)
        //    {
        //        var responseDto = await _contactUSServic.GetContactUsVMById(id);
        //        if (responseDto.StatusCode == Core.Enums.Core.Enums.StatusCode.NotFound)
        //            return View("E404");

        //        return PartialView("_SubjectPopup", responseDto.Data.Subject.ToString());
        //    }

        #region servicenow
        #region functionalities

        [HttpGet]
        public async Task<JsonResult> GetCategories(string source)
        {
            try
            {
                var response = await _serviceNowService.GetCategories(source);
                if (response.StatusCode == Core.Enums.StatusCode.Found)
                    return Json(response.Data);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, source);
            }
            return Json(null);
        }
        [HttpGet]
        public async Task<JsonResult> GetSubCategories(int categoryId)
        {
            try
            {
                var response = await _serviceNowService.GetSubCategories(categoryId);
                if (response.StatusCode == Core.Enums.StatusCode.Found)
                    return Json(response.Data);

            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, categoryId);
            }
            return Json(null);
        }
        [HttpPost]
        public async Task<ActionResult> CreateTicket(TicketVM model)
        {
            ResponseDto responseModel = new ResponseDto();
            try
            {
                var validator = new TicketVMValidator();
                var validationResult = validator.Validate(model);
                if (!validationResult.IsValid || (!User.Identity.IsAuthenticated && string.IsNullOrEmpty(Request.Form["g-recaptcha-response"])))
                {
                    PrepareInvalidModeErrors(responseModel, validationResult);
                    return Json(responseModel);
                }

                ApplicationUser applicationUser = null;
                if (User.Identity.IsAuthenticated)
                {
                    applicationUser = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (applicationUser.PhoneNumber.Substring(0, 2) != "05")
                    {
                        responseModel.StatusCode = Core.Enums.StatusCode.InvalidModel;
                        responseModel.ValidationErrors.Add(new ValidationErrorModel()
                        {
                            ErrorMessage = $"{Resource.MobileNumber} {Resource.StartWith_05}",
                            PropertyName = "MobileNumber"
                        });
                        return Json(responseModel);
                    }
                }
                else
                {
                    if (model.MobileNumber.Length == 10 && model.MobileNumber.Substring(0, 1) == "0")
                    {
                        //05xxxxxxxx
                        model.MobileNumber = $"+966{model.MobileNumber.Substring(1, model.MobileNumber.Length - 1)}";
                    }
                    else if (model.MobileNumber.Length == 9 && model.MobileNumber.Substring(0, 1) == "5")
                    {
                        //5xxxxxxxx
                        model.MobileNumber = $"+966{model.MobileNumber}";
                    }
                    else if (model.MobileNumber.Length == 12 && model.MobileNumber.Substring(0, 4) == "9665")
                    {
                        //9665xxxxxxxx
                        model.MobileNumber = $"+{model.MobileNumber}";
                    }
                    else
                    {
                        //Invalid

                        responseModel.StatusCode = Core.Enums.StatusCode.InvalidModel;
                        responseModel.ValidationErrors.Add(new ValidationErrorModel()
                        {
                            ErrorMessage = $"{Resource.MobileNumber} {Resource.Invalid}",
                            PropertyName = "MobileNumber"
                        });

                        return Json(responseModel);
                    }
                }

                var response = await _serviceNowService.CreateTicket(model, Request.Form.Files, applicationUser);
                if (response.StatusCode == Core.Enums.StatusCode.OK)
                    return Json(new ResponseDto { Message = $"{Resource.CreateTicketSucceeded} - ({response.Data})", StatusCode = Core.Enums.StatusCode.OK });
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, model);

                responseModel.StatusCode = Core.Enums.StatusCode.InternalError;
                responseModel.ValidationErrors.Add(new ValidationErrorModel()
                {
                    ErrorMessage = $"{Resource.InternalErrorWhileLoading}",
                });

                return Json(responseModel);
            }

            responseModel.StatusCode = Core.Enums.StatusCode.SaveFailed;
            responseModel.ValidationErrors.Add(new ValidationErrorModel()
            {
                ErrorMessage = $"{Resource.CreateTicketFailure}",
            });
            return Json(responseModel);
        }
        [HttpGet]
        public async Task<ActionResult> GetTicket(string ticketNumber)
        {
            try
            {
                var response = await _serviceNowService.GetTicket(ticketNumber);
                if (response.StatusCode == Core.Enums.StatusCode.Found)
                    return PartialView("_contact/_viewTicket", response.Data);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, ticketNumber);
            }

            return null;

        }
        [HttpPost]
        public ActionResult GetUserTickets(UserTicketVM model)
        {
            try
            {
                var response = _serviceNowService.GetUserTickets(model.Source, model.IdentityNumber).Result;
                if (response.StatusCode == Core.Enums.StatusCode.Found)
                {
                    var ticketList = mapper.Map<IList<UserTicketsResponseDetails>, IList<TicketVM>>(response.Data);
                    var filteredList = ticketList.Where(x => (string.IsNullOrEmpty(model.TicketNumber) || x.TicketNumber == model.TicketNumber) && (model.FromDate == null || (x.CreatedDate.Date >= model.FromDate && x.CreatedDate.Date <= model.ToDate)));
                    return PartialView("_contact/_ticketListContent", model.OrderType == "asc" ? filteredList.OrderBy(x => x.CreatedDate).ToList() : filteredList.OrderByDescending(x => x.CreatedDate).ToList());
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, model);
            }

            return null;
        }

        [HttpPost]
        public async Task<ActionResult> AddCommentToTicket(string sysId, string comment)
        {
            try
            {
                var response = await _serviceNowService.AddCommentToTicket(sysId, comment);
                if (response.StatusCode == Core.Enums.StatusCode.SaveFailed)
                    return Json(new ResponseDto { Message = $"{Resource.UpdateTicketSucceeded}({response.Data})", StatusCode = Core.Enums.StatusCode.OK });
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, new { sysId, comment });
            }

            return Json(new ResponseDto { Message = $"{Resource.InternalErrorWhileLoading}", StatusCode = Core.Enums.StatusCode.SaveFailed });
        }
        #endregion

        //    #region utils
        //    private static HttpClient CreateHttpClient()
        //    {
        //        var client = new HttpClient();
        //        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ServiceNowBaseUrl"].ToString());
        //        client.DefaultRequestHeaders.Authorization =
        //            new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
        //            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ConfigurationManager.AppSettings["ServiceNowUserName"].ToString()}:{ConfigurationManager.AppSettings["ServiceNowPassword"].ToString()}")));
        //        return client;
        //    }
        //    [AllowAnonymous]
        //    [HttpPost]
        //    public string SetImageOnChange()
        //    {
        //        var photoFile = Request.Files[0];
        //        string content = "Invalid Extension";
        //        if (photoFile.ContentType.ToLower().Contains("jpg")
        //            || photoFile.ContentType.ToLower().Contains("jpeg")
        //            || photoFile.ContentType.ToLower().Contains("png")
        //            || photoFile.ContentType.ToLower().Contains("gif"))
        //        {
        //            byte[] imageByte = null;
        //            BinaryReader rdr = new BinaryReader(photoFile.InputStream);
        //            imageByte = rdr.ReadBytes((int)photoFile.ContentLength);
        //            if (photoFile.ContentLength > 4000000)
        //            {
        //                content = "Size Exceeds 4 MB";
        //                return content;
        //            }
        //            content = "data:image/jpg;base64," + Convert.ToBase64String(imageByte, 0, imageByte.Length);
        //        }
        //        return content;
        //    }
        //    #endregion
        #endregion
    }
}
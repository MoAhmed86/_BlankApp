using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Services;
using Core.DTOs.Identity;
using Core.DTOs;
using Core;
using Core.Resources;
using Microsoft.Extensions.Options;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using UI.Models;
using Core.DTOs.Search;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAccountService accountService;
        private readonly ICommonFunctions commonFunctions;
        private readonly IOptions<Configuration> config;
        private readonly IMapper mapper;
        private readonly ILookupItemService lookupItemService;
        private readonly RoleManager<ApplicationRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAccountService accountService, ICommonFunctions commonFunctions,
            IOptions<Configuration> config, IMapper mapper, ILookupItemService lookupItemService, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.accountService = accountService;
            this.commonFunctions = commonFunctions;
            this.config = config;
            this.mapper = mapper;
            this.lookupItemService = lookupItemService;
            this.roleManager = roleManager;
        }

        public IActionResult LogIn()
        {
            // get reCAPTHCA key from appsettings.json
            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(LoginModel model, string returnUrl)
        {
            // get reCAPTHCA key from appsettings.json
            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;
            try
            {

                if (ModelState.IsValid && await commonFunctions.ReCaptchaPassed(Request.Form["g-recaptcha-response"], config.Value.GoogleReCaptchaSecretKey))
                {
                    var userObj = await userManager.FindByEmailAsync(model.UserName);
                    if (userObj != null)
                    {
                        var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                        if (result.Succeeded)
                        {
                            await signInManager.UserManager.UpdateSecurityStampAsync(userObj);

                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);
                            else
                                return RedirectToAction("Index", "Home");
                        }
                        else
                            ModelState.AddModelError("Password", $"{Resource.UserName} {Resource.Or} {Resource.Password} {Resource.Invalid}");
                    }
                    else
                        ModelState.AddModelError("UserName", $"{Resource.UserName} {Resource.NotRegistered}");
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, model);
            }

            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string r, string returnUrl)
        {
            //Check if user is Super Admin
            if (User.Identity.IsAuthenticated && !User.IsInRole(CommonStaticFunctions.Role_SuperAdmin))
                return RedirectToAction("E403","Error");

            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;

            ViewBag.Role = r;

            return View(new RegisterDto());
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterDto model,string role, string returnUrl)
        {
            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;
            try
            {
                if (ModelState.IsValid && await commonFunctions.ReCaptchaPassed(Request.Form["g-recaptcha-response"], config.Value.GoogleReCaptchaSecretKey))
                {
                    int userId= 0;
                    if (User.Identity.IsAuthenticated)
                    {
                        if (User.IsInRole(CommonStaticFunctions.Role_SuperAdmin))
                            role = CommonStaticFunctions.Role_SuperAdmin;

                        userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }

                    var res = await accountService.SaveUserAsync(model,role, userId);

                    if (res.StatusCode == Enums.StatusCode.OK)
                    {
                        bool isSent = commonFunctions.SendEmail(res.Data.Email, null, Resource.CompleteRegistration, Enums.EmailTypes.NewRegistration);

                        if (isSent)
                            TempData["Success"] = Resource.RegisterSucceeded;
                        else
                        {
                            TempData["Success"] = Resource.RegisterSucceeded;
                            TempData["Error"] = Resource.SendMsgToEmailFailed;
                        }

                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("Users");
                    }

                    if (res.StatusCode == Enums.StatusCode.NotFound)
                        ModelState.AddModelError("IdentityNumber", $"{Resource.IdentityNumber} {Resource.NotRegistered}");
                    else if (res.StatusCode == Enums.StatusCode.Duplicated)
                    {
                        if (string.IsNullOrEmpty(res.Message))
                            ModelState.AddModelError("EmailAddress", $"{Resource.Email} {Resource.PreviouslyRegistered}");
                        else
                            ModelState.AddModelError("IdentityNumber", res.Message);
                    }
                    else if (res.StatusCode == Enums.StatusCode.SaveFailed)
                        ModelState.AddModelError("IdentityNumber", $"{Resource.IdentityNumber} {Resource.NotRegistered}");
                    else
                        ModelState.AddModelError("", $"{Resource.InternalErrorWhileLoading}");
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, model);
            }

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);
            RegisterDto model = mapper.Map<RegisterDto>(user);

            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;

            return View("Create", model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RegisterDto model)
        {
            ViewData["ReCaptchaKey"] = config.Value.GoogleReCaptchaSiteKey;
            try
            {
                if (ModelState.IsValid && await commonFunctions.ReCaptchaPassed(Request.Form["g-recaptcha-response"], config.Value.GoogleReCaptchaSecretKey))
                {
                    int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    var res = await accountService.SaveUserAsync(model,string.Empty, userId);

                    if (res.StatusCode == Enums.StatusCode.OK)
                    {
                        TempData["Success"] = Resource.RegisterSucceeded;
                        return RedirectToAction("Users");
                    }

                    if (res.StatusCode == Enums.StatusCode.NotFound || res.StatusCode == Enums.StatusCode.Duplicated || res.StatusCode == Enums.StatusCode.SaveFailed)
                    {
                        if (res.StatusCode == Enums.StatusCode.NotFound)
                            ModelState.AddModelError("IdentityNumber", $"{Resource.IdentityNumber} {Resource.NotRegistered}");
                        else if (res.StatusCode == Enums.StatusCode.Duplicated)
                            ModelState.AddModelError("EmailAddress", $"{Resource.Email} {Resource.PreviouslyRegistered}");
                        else if (res.StatusCode == Enums.StatusCode.SaveFailed)
                            ModelState.AddModelError("IdentityNumber", $"{Resource.IdentityNumber} {Resource.NotRegistered}");
                        else
                            ModelState.AddModelError("IdentityNumber", $"{Resource.InternalErrorWhileLoading}");
                    }

                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, model);
            }

            return View("Create", model);
        }

        [HttpGet]
        public IActionResult Users()
        {
            try
            {
                ViewBag.Roles = roleManager.Roles.ToList();

                IList<LookupItemDto> lkpItems = new List<LookupItemDto>();
                lkpItems.Add(new LookupItemDto()
                {
                    Id = 0,
                    DescAr = Resource.Active,
                });
                lkpItems.Add(new LookupItemDto()
                {
                    Id = 1,
                    DescAr = Resource.InActive,
                });

                ViewBag.Status = lkpItems;
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
            }

            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> UsersPartial(UserSearchModel userSearchModel)
        {
            PagedListModel<ApplicationUser> users = new PagedListModel<ApplicationUser>();

            try
            {
                var currentUser = await userManager.GetUserAsync(User);
                users = await accountService.GetUsers(userSearchModel);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, userSearchModel);
            }

            return PartialView("_Users", users);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Details(int? id = null)
        {
            RegisterDto model = new RegisterDto();
            
            try
            {
                if (!id.HasValue || id.Value == 0)
                    id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var user = await userManager.GetUserAsync(User);
                model = mapper.Map<RegisterDto>(user);

                ViewBag.emp = await userManager.IsInRoleAsync(user, CommonStaticFunctions.Role_Employee);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                return RedirectToAction("Error", "E500");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var response = await accountService.DeleteUserAsync(id);
                if (response.StatusCode == Enums.StatusCode.Activated)
                    TempData["Success"] = Resource.ActivatingUserSucceeded;
                else if (response.StatusCode == Enums.StatusCode.Deactivated)
                    TempData["Success"] = Resource.DeleteUserSucceeded;
                else
                    return RedirectToAction("E500", "Error");
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, id);
                return RedirectToAction("E500", "Error");
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ForgotPasswordModel model = new ForgotPasswordModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        var passwordRestLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                        bool isSent = commonFunctions.SendEmail(model.Email, passwordRestLink, $"{Resource.Forgot} {Resource.Password}", Enums.EmailTypes.NewPassword);

                        if (isSent)
                            TempData["Success"] = Resource.MessageSentToEmail;
                        else
                            TempData["Error"] = Resource.SendMsgToEmailFailed;
                    }
                    else
                        TempData["Error"] = Resource.EmailNotFound;
                }
                else
                    PrepareInvalidModelErrors(response);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Resource.InternalErrorWhileLoading;
                commonFunctions.LogError(ex, model);
            }

            return View(model);
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordModel();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", $"{Resource.Token} {Resource.Invalid}");
            }
            else
            {
                model.Email = email;
                model.Token = token;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> ResetPassword(ResetPasswordModel model)
        {
            ResponseDto responseModel = new ResponseDto();

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                        if (result.Succeeded)
                        {
                            await userManager.ConfirmEmailAsync(user, model.Token);
                            responseModel.StatusCode = Enums.StatusCode.OK;
                            TempData["Success"] = Resource.ResetPasswordSucceededMsg;
                        }
                        else
                        {
                            //TODO: search for error codes
                            responseModel.StatusCode = Enums.StatusCode.InvalidModel;
                            responseModel.ValidationErrors.Add(new ValidationErrorModel { ErrorMessage = Resource.InvalidToken, PropertyName = "" });

                            //foreach (var error in result.Errors)
                            //{
                            //    responseModel.ValidationErrors.Add(new ValidationErrorModel { ErrorMessage = error.Description, PropertyName = error.Code });
                            //}

                        }
                    }
                    else
                    {
                        responseModel.StatusCode = Enums.StatusCode.InvalidModel;
                        responseModel.ValidationErrors.Add(new ValidationErrorModel { ErrorMessage = $"{Resource.Email} {Resource.NotRegistered}", PropertyName = "Email" });
                    }
                }
                else
                    PrepareInvalidModelErrors(responseModel);
            }
            catch (Exception ex)
            {
                responseModel.StatusCode = Enums.StatusCode.InternalError;
                commonFunctions.LogError(ex, model);
            }

            return Json(responseModel);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private void PrepareInvalidModelErrors(ResponseDto response)
        {
            response.StatusCode = Core.Enums.StatusCode.InvalidModel;

            foreach (var k in ModelState.Keys)
            {
                foreach (var error in ModelState[k].Errors)
                {
                    response.ValidationErrors.Add(new ValidationErrorModel()
                    {
                        ErrorMessage = error.ErrorMessage,
                        PropertyName = k
                    });
                }
            }
        }
    }
}
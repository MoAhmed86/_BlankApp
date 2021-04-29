using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using Core.DTOs;
using Core.DTOs.Identity;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<JwtConfig> config;
        private readonly ICommonFunctions commonFunctions;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthController(IOptions<JwtConfig> config,ICommonFunctions commonFunctions,UserManager<ApplicationUser> userManager
            ,SignInManager<ApplicationUser> signInManager)
        {
            this.config = config;
            this.commonFunctions = commonFunctions;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        //[Route("Login")]
        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult Login(ClientModel model)
        //{
        //    if (AuthenticateUser(model))
        //    {
        //        ResponseHttp res = new ResponseHttp();
        //        res.StatusCode = System.Net.HttpStatusCode.OK;
        //        res.Message = GenerateToken();
        //        return Ok(res);
        //    }

        //    return Unauthorized();
        //}

        //private string GenerateToken()
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(config.Value.Issuer,
        //      config.Value.Audience,
        //      null,
        //      expires: DateTime.Now.AddMinutes(120),
        //      signingCredentials: credentials);


        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //private bool AuthenticateUser(ClientModel model)
        //{
        //    if (model.UserName == config.Value.RawaafedUserName && model.Password == config.Value.RawaafedPassword)
        //        return true;

        //    return false;
        //}

        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(ClientModel model)
        {
            try
            {
                var response = await AuthenticateUser(model);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ResponseHttp res = new ResponseHttp();
                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Message = GenerateToken(response.Data);
                    return Ok(res);
                }
                else
                    return Unauthorized(response.ValidationErrors);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, model);
                return StatusCode(500);
            }
        }


        private string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config.Value.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim("id", user.Id.ToString())
            };

            //var userClaims = await userManager.GetClaimsAsync(user);
            //claims.AddRange(userClaims);

            //var userRoles = await userManager.GetRolesAsync(user);
            //foreach (var userRole in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, userRole));
            //    var role = await _roleManager.FindByNameAsync(userRole);
            //    if (role == null) continue;
            //    var roleClaims = await _roleManager.GetClaimsAsync(role);

            //    foreach (var roleClaim in roleClaims)
            //    {
            //        if (claims.Contains(roleClaim))
            //            continue;

            //        claims.Add(roleClaim);
            //    }
            //}

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(new TimeSpan(0, 30, 0)),//30 min
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<ResponseHttp<ApplicationUser>> AuthenticateUser(ClientModel model)
        {
            ResponseHttp<ApplicationUser> response = new ResponseHttp<ApplicationUser>();

            if (ModelState.IsValid)
            {
                var userObj = await userManager.FindByNameAsync(model.UserName);
                if (userObj != null)
                {
                    var result = await signInManager.CheckPasswordSignInAsync(userObj, model.Password, false);

                    if (result.Succeeded)
                    {
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Data = userObj;
                    }
                    else
                    {
                        response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                        response.ValidationErrors.Add(new ValidationErrorModel { ErrorMessage = $"{Resource.UserName} {Resource.Or} {Resource.Password} {Resource.Invalid}", PropertyName = "Password" });
                    }
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.ValidationErrors.Add(new ValidationErrorModel { ErrorMessage = $"{Resource.UserName} {Resource.Invalid}", PropertyName = "UserName" });
                }
            }
            else
                PrepareInvalidModelErrors(response);
            return response;
        }


        private void PrepareInvalidModelErrors(ResponseHttp response)
        {
            response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            response.ValidationErrors = ModelState.Values.SelectMany(m => m.Errors).Select(e => new ValidationErrorModel() { ErrorMessage = e.ErrorMessage }).ToList();
        }
    }
}
using System.Security.Claims;
using Application.DTOs.Account;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SiaRoute.WebApp.Helpers;
using SiaRoute.WebApp.Models;

namespace SiaRoute.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService; 
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public AccountController(IAccountService accountService, IAuthenticatedUserService authenticatedUserService)
        {
            _accountService = accountService;
            _authenticatedUserService = authenticatedUserService;
        }
        
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("account/authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var user= await _accountService.AuthenticateAsync(request, GenerateIPAddress());
            if (user.Succeeded)
            {
               
                var token = JwtUtils.ConvertJwtStringToJwtSecurityToken(user.Data.JWToken);
                var decodeToken= JwtUtils.DecodeJwt(token);
                var claims = new List<Claim>();
                foreach (var claim in decodeToken.Claims)
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }
                var identity= new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                return Ok(user);
            }
            return Unauthorized();
        }
        [HttpPost("account/kayit-olustur")]
        public async Task<IActionResult> AccountRegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _accountService.RegisterAsync(request, origin));
        }
        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        public IActionResult Logout()
        {
            _accountService.LogOut();
            return RedirectToAction("Login");
        }
        [Route("yetkisiz-sayfa")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ProfileSettings()
        {
            var userProfile =await _accountService.GetByUserProfile(_authenticatedUserService.UserId);
            return View(userProfile);
        }

        [HttpPost("account/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePassword model)
        {
            string userId = _authenticatedUserService.UserId;
            var resetPassword =await _accountService.ResetPassword(new ResetPasswordRequest()
            {
                ConfirmPassword = model.ConfirmPassword,
                Password = model.Password,
                CurrentPassword = model.CurrentPassword,
                Id = userId
            });
            return Ok(resetPassword);
        }
    }
}

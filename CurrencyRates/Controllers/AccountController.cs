using CurrencyRates.Interfaces;
using CurrencyRates.Models;
using CurrencyRates.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Currency.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _repository;
        public AccountController(IAccountRepository repository)
        {
            this._repository = repository;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnURL)
        {
            if (ModelState.IsValid)
            {
                Response<UserModel> user = _repository.ValidateUser(model);
                if (user.Code == 1)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Data.UserName),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Data.RoleGUID),
                        new Claim("UserFIO",user.Data.Name)
                    };
                    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                    
                    return this.RedirectToAction("Index", "ExchangeRate");
                }
                ModelState.AddModelError("", "Неверный логин или пароль!");
            }
            return this.View(model);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public string ChangePassword(string password)
        {
            try
            {
                _repository.ChangePassword(password, User.Identity.Name);
                return "true";
                   
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
    }
}

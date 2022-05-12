using CurrencyRates.Interfaces;
using CurrencyRates.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Controllers
{
    [Authorize(Roles= "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2")]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _repository;
        public AdminController(IAdminRepository repository)
        {
            this._repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public string AddUser(NewUser model)
        {
            try
            {                
                return _repository.AddUser(model);
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        public IActionResult Roles()
        {
            return View(_repository.GetRoles());
        }
        public IActionResult Users()
        {
            return View(_repository.GetUsers());
        }

        [HttpGet]
        public IActionResult ChangeRoleUser()
        {
            return View();
        }
        [HttpPost]
        public string ChangeRoleUser(int userId, int roleId)
        {
            try
            {
                _repository.ChangeRoleUser(userId, roleId);
                return "true";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        [HttpGet]
        public IActionResult ResetPasswordUser()
        {
            return View();
        }
        [HttpPost]
        public string ResetPasswordUser(int userId)
        {
            return _repository.ResetPassword(userId);
        }
    }
}

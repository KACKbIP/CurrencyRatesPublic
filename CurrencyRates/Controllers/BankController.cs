using CurrencyRates.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Controllers
{
    [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
    public class BankController : Controller
    {
        private readonly IBankRepository _repository;
        public BankController(IBankRepository repository)
        {
            this._repository = repository;
        }
        public IActionResult Index()
        {
            return View(_repository.GetBanks());
        }
        [HttpGet]
        public IActionResult AddBank()
        {
            return View();
        }
        [HttpPost]
        public string AddBank(string name, int processingId, bool withPercent )
        {
            try
            {                
                return _repository.InsertBank(name, processingId, withPercent); 
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        public void UpdateBank(bool isActive, int id)
        {
            _repository.UpdateBank(isActive, id);
        }
        public void DeleteBank(int id)
        {
            _repository.DeleteBank(id);
        }
        public IActionResult List()
        {
            return View(_repository.GetBanks());
        }
    }
}

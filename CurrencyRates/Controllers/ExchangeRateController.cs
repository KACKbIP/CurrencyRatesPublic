using CurrencyRates.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Controllers
{
    [Authorize]
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRateRepository _repository;
        public ExchangeRateController(IExchangeRateRepository repository)
        {
            this._repository = repository;
        }
        [Authorize]
        public IActionResult Index(int? currencyInId, int? currencyOutId)
        {
            TempData["InId"] = currencyInId;
            TempData["OutId"] = currencyOutId;
            return View(_repository.GetExchangeRate(currencyInId, currencyOutId).Where(m => m.WithPercent == false).ToList());
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        [Authorize]
        public IActionResult IndexWithPercent(int? currencyInId, int? currencyOutId)
        {
            TempData["InId"] = currencyInId;
            TempData["OutId"] = currencyOutId;
            return View(_repository.GetExchangeRate(currencyInId, currencyOutId).Where(m=>m.WithPercent==true).ToList());
        }
        [HttpGet]
        public IActionResult History(string from, string to)
        {
            if (from != null && to != null)
            {
                DateTime fromDate = Convert.ToDateTime(from);
                DateTime toDate = Convert.ToDateTime(to);
                ViewBag.From = fromDate.ToString("dd.MM.yyyy");
                ViewBag.To = toDate.ToString("dd.MM.yyyy");
                return View(_repository.GetLogExchangeRate(fromDate, toDate));
            }
            else
                return View();
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        [HttpPost]
        public string Update(int id, double selling, double purch, bool isManulInput, double percent)
        {
            try
            {
                if(selling <= 0)
                    throw new Exception("Сумма продажи равна нулю либо меньше нуля!");
                if (purch <= 0)
                    throw new Exception("Сумма покупки равна нулю либо меньше нуля!");
                if (purch < selling)
                    throw new Exception("Сумма покупки меньше чем сумма продажи!");
                
                return _repository.Update(id, selling, purch, percent, isManulInput, this.User.Claims.Where(c=>c.Type=="UserFIO").FirstOrDefault().Value);
            }
            catch (Exception e)
            { return e.Message; }
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        [HttpGet]
        public IActionResult AddExchangeRate()
        {
            return View();
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        [HttpPost]
        public string AddExchangeRate(int bankId, int currencyInId, int currencyOutId, double selling, double purch, double percent)
        {            
            try
            {
                if(currencyInId==currencyOutId)
                    throw new Exception("Укажите разные валюты!");
                if (selling <= 0)
                    throw new Exception("Сумма продажи равна нулю либо меньше нуля!");
                if (purch <= 0)
                    throw new Exception("Сумма покупки равна нулю либо меньше нуля!");
                if (purch < selling)
                    throw new Exception("Сумма покупки меньше чем сумма продажи!");
                

                return _repository.InsertExchangeRate(bankId, currencyInId, currencyOutId, selling, purch, percent, this.User.Claims.Where(c => c.Type == "UserFIO").FirstOrDefault().Value);
            }
            catch (Exception e)
            { return e.Message; }
        }
    }
}

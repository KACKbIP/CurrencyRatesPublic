using CurrencyRates.Interfaces;
using CurrencyRates.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Controllers
{
    [Authorize]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyRepository _repository;
        public CurrencyController(ICurrencyRepository repository)
        {
            this._repository = repository;
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        public IActionResult Index()
        {
            return View(_repository.GetCurrencies());
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        public void Delete(int id)
        {
            _repository.DeleteCurrency(id);
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        [HttpGet]
        public IActionResult AddCurrency()
        {
            return View();
        }
        [Authorize(Roles = "592F3B0D-2C64-4D42-A0B1-8DB14E424FA2,BB938783-4128-472D-8BCD-E61BB5D854C7")]
        [HttpPost]
        public string AddCurrency(string name, string iso)
        {
            return (_repository.InsertCurrency(name, iso));
        }
        public IActionResult ListIn(bool? isExchangeRate)
        {
            if(isExchangeRate==null)
            return View(_repository.GetCurrencies());
            else
            {
                List<CurrencyModel> list = _repository.GetCurrencies();
                list.Add(new CurrencyModel
                {
                    Id = 0,
                    Name = "Все курсы",
                    ISO = "Все курсы"
                });
                return View(list);
            }
        }
        public IActionResult ListOut(bool? isExchangeRate)
        {
            if (isExchangeRate == null)
                return View(_repository.GetCurrencies());
            else
            {
                List<CurrencyModel> list = _repository.GetCurrencies();
                list.Add(new CurrencyModel
                {
                    Id = 0,
                    Name = "Все курсы",
                    ISO = "Все курсы"
                });
                return View(list);
            }
        }
    }
}

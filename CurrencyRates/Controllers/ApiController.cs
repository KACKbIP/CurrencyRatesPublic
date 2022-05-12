using CurrencyRates.Interfaces;
using CurrencyRates.Models;
using CurrencyRates.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class ApiController : ControllerBase
    {
        private readonly IApiRepository _repository;
        public ApiController(IApiRepository repository)
        {
            this._repository = repository;
        }
        public Response<List<string>> UpdateCurrencyRates()
        {
            Response< List<string>> response = new Response<List<string>>();
            try
            {
                response.Code = 0;
                response.Message = "success";
                response.Data = new List<string>();
                response.Data.Add($"Eurasian Bank: {_repository.EuBankCurrency()}");
                //response.Data.Add($"Alfa Bank: {_repository.AlfaCurriency()}");
                //response.Data.Add($"Kaspi Bank: {_repository.KaspiCurriency()}");
                response.Data.Add($"Halyk Bank: {_repository.HalykCurriency()}");
                response.Data.Add($"NationalBank: {_repository.NationalBankCurriency()}");
                response.Data.Add($"OptimaBank: {_repository.OptimaBank()}");
                //response.Data.Add($"Dos CredoBank: {_repository.DosCredoCurriency()}");
                response.Data.Add($"Dos CredoBank Наличные: {_repository.DosCredoCurriencyNal()}");
                response.Data.Add($"CBU: {_repository.CBUCurriency()}");
                response.Data.Add($"FMFB: {_repository.FMFMCurriency()}");
                response.Data.Add($"Cross course: {_repository.CrossCourse()}");
                response.Data.Add($"Alif Bank: {_repository.AlifBank()}");
                response.Data.Add($"NationalBankTJS: {_repository.NationalBankTJS()}");
                response.Data.Add($"NationalBankMDL: {_repository.NationalBankMDL()}");
                response.Data.Add($"VoltonBank: {_repository.VoltonBank()}");
                response.Data.Add($"RunPay Bank: {_repository.AgroindbankMDL()}");
                response.Data.Add($"Payvand Bank: {_repository.PayvandTJS()}");
            }
            catch(Exception e)
            {
                response.Code = -1;
                response.Message = "error";
                response.Data = new List<string>();
                response.Data.Add(e.Message);
            }
            return response;
        }
        public Response<dynamic> GetCurrencyRates()
        {
            Response<dynamic> response = new Response<dynamic>();
            try
            {
                response.Code = 0;
                response.Message = "success";
                response.Data = _repository.GetCurrencyRatesForProcessing();
            }
            catch(Exception e)
            {
                response.Code = -1;
                response.Message = e.Message;
            }
            return response;
        }
    }
}

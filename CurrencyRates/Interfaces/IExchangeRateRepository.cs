using CurrencyRates.Models.ExchangeRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Interfaces
{
    public interface IExchangeRateRepository
    {
        List<ExchangeModel> GetExchangeRate(int? currencyInId, int? currencyOutId);
        string Update(int id, double selling, double purch, double percent, bool isManulInput, string user);
        string InsertExchangeRate(int bankId, int currencyInId, int currencyOutId, double selling, double purch, double percent, string user);
        List<ExchangeModel> GetLogExchangeRate(DateTime? from, DateTime? to);
    }
}

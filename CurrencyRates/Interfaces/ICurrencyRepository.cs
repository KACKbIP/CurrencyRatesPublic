using CurrencyRates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Interfaces
{
    public interface ICurrencyRepository
    {
        List<CurrencyModel> GetCurrencies();
        void DeleteCurrency(int id);
        string InsertCurrency(string name, string iso);
    }
}

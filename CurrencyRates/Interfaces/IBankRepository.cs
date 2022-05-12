using CurrencyRates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Interfaces
{
    public interface IBankRepository
    {
        List<BankModel> GetBanks();
        string InsertBank(string name, int processingId, bool withPercent);
        void UpdateBank(bool isActive, int id);
        BankModel GetBank(int id);
        void DeleteBank(int id);
    }
}

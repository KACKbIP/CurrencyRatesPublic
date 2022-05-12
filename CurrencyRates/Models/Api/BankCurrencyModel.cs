using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models.Api
{
    public class BankCurrencyModel
    {
        public string CurrencyName { get; set; }
        public float Selling { get; set; }
        public float Purchanse { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models.ExchangeRate
{
    public class ExchangeModel
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public bool WithPercent { get; set; }
        public int CurrencyInId { get; set; }
        public string CurrencyInName { get; set; }
        public string CurrencyInISO { get; set; }
        public int CurrencyOutId { get; set; }
        public string CurrencyOutName { get; set; }
        public string CurrencyOutISO { get; set; }
        public double SellingRate { get; set; }
        public double PurchanseRate { get; set; }
        public bool IsManualInput { get; set; }
        public bool IsUpdateAuto { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UserFIO { get; set; }
        public double Percent { get; set; }
    }
}

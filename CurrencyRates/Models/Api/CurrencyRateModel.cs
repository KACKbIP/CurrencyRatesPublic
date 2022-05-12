using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models.Api
{
    public class CurrencyRateModel
    {
        [JsonProperty("BankId")]
        public int BankId { get; set; }
        [JsonProperty("Rates")]
        public List<Rate> Rates { get; set; }
        public class Rate
        {
            [JsonProperty("CurrencyInISO")]
            public string CurrencyInISO { get; set; }
            [JsonProperty("CurrencyOutISO")]
            public string CurrencyOutISO { get; set; }
            [JsonProperty("SellingRate")]
            public double SellingRate { get; set; }
            [JsonProperty("PurchanseRate")]
            public double PurchanseRate { get; set; }
        }
    }
}

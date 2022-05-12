using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models.Api
{
    public class NationalBank
    {
        public string ISO { get; set; }
        public double Value { get; set; }
        public int Quantity { get; set; }
    }
}

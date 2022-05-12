using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models
{
    public class BankModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProcessingId { get; set; }
        public bool IsActive { get; set; }
        public bool WithPercent { get; set; }
    }
}

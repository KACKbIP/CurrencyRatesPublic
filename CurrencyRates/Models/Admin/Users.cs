using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models.Admin
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
    }
}

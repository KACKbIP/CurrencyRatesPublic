using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Models.Account
{
    public class LoginModel
    {
        [Required, Display(Name = "Логин"), MaxLength(50)]
        public string Login { get; set; }
        [Required, Display(Name = "Пароль"), DataType(DataType.Password), MaxLength(50)]
        public string Password { get; set; }
        [Display(Name = "Запомнить")]
        public bool IsRemember { get; set; }
    }
}

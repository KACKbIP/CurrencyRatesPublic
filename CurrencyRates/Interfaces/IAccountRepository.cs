using CurrencyRates.Models;
using CurrencyRates.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Interfaces
{
    public interface IAccountRepository
    {
        Response<UserModel> ValidateUser(LoginModel model);
        void ChangePassword(string password, string username);
    }
}

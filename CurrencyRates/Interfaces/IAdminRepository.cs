using CurrencyRates.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Interfaces
{
    public interface IAdminRepository
    {
        string AddUser(NewUser model);
        List<Roles> GetRoles();
        void ChangeRoleUser(int userId, int roleId);
        string ResetPassword(int userId);
        List<Users> GetUsers();
    }
}

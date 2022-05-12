using CurrencyRates.Interfaces;
using CurrencyRates.Models;
using CurrencyRates.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;
        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }

        public Response<UserModel> ValidateUser(LoginModel model)
        {
            Response<UserModel> response = new Response<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("dbo.ValidateUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userName", model.Login);
                    cmd.Parameters.AddWithValue("@password", HelperRepository.EncryptPassword(model.Password));
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        throw new Exception("Неверный логин или пароль!");
                    }

                    response.Data = new UserModel();
                    while (reader.Read())
                    {

                        response.Data.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                        response.Data.Name = reader["Name"] != DBNull.Value ? Convert.ToString(reader["Name"]) : string.Empty;
                        response.Data.UserName = reader["UserName"] != DBNull.Value ? Convert.ToString(reader["UserName"]) : string.Empty;
                        response.Data.RoleGUID = reader["RoleGUID"] != DBNull.Value ? Convert.ToString(reader["RoleGUID"]) : string.Empty;
                    }
                    response.Code = 1;
                    response.Message = "success";
                }
                catch (Exception ex)
                {
                    response.Code = -1;
                    response.Message = ex.Message;
                }
            }
            return response;
        }
        [Authorize]
        public void ChangePassword(string password, string username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.ChangePassword", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userName", username);
                cmd.Parameters.AddWithValue("@password", HelperRepository.EncryptPassword(password));
                cmd.ExecuteNonQuery();
            }
        }
    }
}

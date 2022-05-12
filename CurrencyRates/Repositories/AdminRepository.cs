using CurrencyRates.Interfaces;
using CurrencyRates.Models.Admin;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;
        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }
        public string AddUser(NewUser model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.AddUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", model.Surname + " " + model.Name);
                cmd.Parameters.AddWithValue("@login", model.Login);
                cmd.Parameters.AddWithValue("@password", HelperRepository.EncryptPassword(model.Password));
                cmd.Parameters.AddWithValue("@roleId", model.RoleId);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }
        public List<Roles> GetRoles()
        {
            List<Roles> roles = new List<Roles>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.GetRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Roles role = new Roles();
                    role.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    role.Name = reader["Name"] != DBNull.Value ? Convert.ToString(reader["Name"]) : string.Empty;
                    roles.Add(role);
                }
            }
            return roles;
        }
        public List<Users> GetUsers()
        {
            List<Users> users = new List<Users>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.GetUsers", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Users user = new Users();
                    user.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    user.Name = reader["Name"] != DBNull.Value ? Convert.ToString(reader["Name"]) : string.Empty;
                    user.UserName = reader["UserName"] != DBNull.Value ? Convert.ToString(reader["UserName"]) : string.Empty;
                    user.RoleId = reader["roleId"] != DBNull.Value ? Convert.ToInt32(reader["roleId"]) : 0;
                    users.Add(user);
                }
            }
            return users;
        }
        public void ChangeRoleUser(int userId, int roleId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.ChangeRoleUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@roleId", roleId);
                cmd.ExecuteNonQuery();
            }
        }
        public string ResetPassword(int userId)
        {
            string[] randoms = new string[10];
            randoms[0] = "AV";
            randoms[1] = "xf";
            randoms[2] = "d12";
            randoms[3] = "fbt";
            randoms[4] = "f4sa";
            randoms[5] = "a!z";
            randoms[6] = "s@j7";
            randoms[7] = "3d";
            randoms[8] = "!f6";
            randoms[9] = "fF";
            Random r = new Random();
            string randsror = randoms[r.Next(0, randoms.Length - 1)];

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.ResetPassword", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@password", HelperRepository.EncryptPassword(randsror));
                cmd.ExecuteNonQuery();
            }
            return randsror;
        }
    }
}

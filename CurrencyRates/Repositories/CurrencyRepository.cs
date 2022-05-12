using CurrencyRates.Interfaces;
using CurrencyRates.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRates.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly string _connectionString;
        public CurrencyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }

        public List<CurrencyModel> GetCurrencies()
        {
            List<CurrencyModel> models = new List<CurrencyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.GetCurrencies", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CurrencyModel model = new CurrencyModel();
                    model.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    model.Name = reader["Name"] != DBNull.Value ? Convert.ToString(reader["Name"]) : string.Empty;
                    model.ISO = reader["ISO"] != DBNull.Value ? Convert.ToString(reader["ISO"]) : string.Empty;

                    models.Add(model);
                }
            }
            return models;
        }

        public void DeleteCurrency(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.DeleteCurrency", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        public string InsertCurrency(string name, string iso)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.InsertCurrency", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@iso", iso);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }
    }
}

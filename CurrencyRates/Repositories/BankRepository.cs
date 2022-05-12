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
    public class BankRepository : IBankRepository
    {
        private readonly string _connectionString;
        public BankRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }
        public BankModel GetBank(int id)
        {
            BankModel bank = new BankModel();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.GetBank", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    
                    bank.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    bank.Name = reader["Name"] != DBNull.Value ? Convert.ToString(reader["Name"]) : string.Empty;
                    bank.ProcessingId = reader["ProcessingId"] != DBNull.Value ? Convert.ToInt32(reader["ProcessingId"]) : 0;
                    bank.IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : false;
                    bank.WithPercent = reader["WithPercent"] != DBNull.Value ? Convert.ToBoolean(reader["WithPercent"]) : false;
                }
            }
            return bank;
        }
        public List<BankModel> GetBanks()
        {
            List<BankModel> models = new List<BankModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.GetBanks", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    BankModel bank = new BankModel();
                    bank.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    bank.Name = reader["Name"] != DBNull.Value ? Convert.ToString(reader["Name"]) : string.Empty;
                    bank.ProcessingId = reader["ProcessingId"] != DBNull.Value ? Convert.ToInt32(reader["ProcessingId"]) : 0;
                    bank.IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : false;
                    bank.WithPercent = reader["WithPercent"] != DBNull.Value ? Convert.ToBoolean(reader["WithPercent"]) : false; 

                    models.Add(bank);
                }
            }
            return models;
        }
        public string InsertBank(string name, int processingId, bool withPercent)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.InsertBank", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@processingId", processingId);
                cmd.Parameters.AddWithValue("@withPercent", withPercent);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }
        public void UpdateBank(bool isActive, int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.UpdateBank", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@isActive", isActive);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteBank(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.DeleteBank", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

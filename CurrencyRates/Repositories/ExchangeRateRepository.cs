using CurrencyRates.Interfaces;
using CurrencyRates.Models.ExchangeRate;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CurrencyRates.Models;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CurrencyRates.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly string _connectionString;
        public ExchangeRateRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
        }
        public List<ExchangeModel> GetExchangeRate(int? currencyInId, int? currencyOutId)
        
        {
            List<ExchangeModel> exchanges = new List<ExchangeModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.GetExchangeRates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@currencyInId", currencyInId);
                cmd.Parameters.AddWithValue("@currencyOutId", currencyOutId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ExchangeModel exchange = new ExchangeModel();
                    exchange.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                    exchange.BankId = reader["BankId"] != DBNull.Value ? Convert.ToInt32(reader["BankId"]) : 0;
                    exchange.BankName = reader["BankName"] != DBNull.Value ? Convert.ToString(reader["BankName"]) : string.Empty;
                    exchange.WithPercent = reader["WithPercent"] != DBNull.Value ? Convert.ToBoolean(reader["WithPercent"]) : false;
                    exchange.Percent = reader["Percent"] != DBNull.Value ? Convert.ToDouble(reader["Percent"]) : 0;
                    exchange.CurrencyInId = reader["CurrencyInId"] != DBNull.Value ? Convert.ToInt32(reader["CurrencyInId"]) : 0;
                    exchange.CurrencyInName = reader["CurrencyInName"] != DBNull.Value ? Convert.ToString(reader["CurrencyInName"]) : string.Empty;
                    exchange.CurrencyInISO = reader["CurrencyInISO"] != DBNull.Value ? Convert.ToString(reader["CurrencyInISO"]) : string.Empty;
                    exchange.CurrencyOutId = reader["CurrencyOutId"] != DBNull.Value ? Convert.ToInt32(reader["CurrencyOutId"]) : 0;
                    exchange.CurrencyOutName = reader["CurrencyOutName"] != DBNull.Value ? Convert.ToString(reader["CurrencyOutName"]) : string.Empty;
                    exchange.CurrencyOutISO = reader["CurrencyOutISO"] != DBNull.Value ? Convert.ToString(reader["CurrencyOutISO"]) : string.Empty;
                    exchange.SellingRate = reader["SellingRate"] != DBNull.Value ? Convert.ToDouble(reader["SellingRate"]) : 0;
                    exchange.PurchanseRate  = reader["PurchanseRate"] != DBNull.Value ? Convert.ToDouble(reader["PurchanseRate"]) : 0;
                    exchange.IsManualInput = reader["IsManualInput"] != DBNull.Value ? Convert.ToBoolean(reader["IsManualInput"]) : false;
                    exchange.IsUpdateAuto = reader["IsUpdateAuto"] != DBNull.Value ? Convert.ToBoolean(reader["IsUpdateAuto"]) : false;
                    exchange.UpdateDate = reader["UpdateDate"] != DBNull.Value ? Convert.ToDateTime(reader["UpdateDate"]) : DateTime.MinValue;
                    exchange.UserFIO = reader["UserFIO"] != DBNull.Value ? Convert.ToString(reader["UserFIO"]) : string.Empty;
                    exchanges.Add(exchange);
                }
            }
            return exchanges;
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

                    models.Add(bank);
                }
            }
            return models;
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
        public string Update(int id, double selling, double purch, double percent, bool isManulInput, string user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.UpdateExchangeRates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@sell", selling);
                cmd.Parameters.AddWithValue("@purch", purch);
                cmd.Parameters.AddWithValue("@percent", percent);
                cmd.Parameters.AddWithValue("@isManualInput", isManulInput);
                cmd.Parameters.AddWithValue("@user", user);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public string InsertExchangeRate(int bankId, int currencyInId, int currencyOutId, double selling, double purch, double percent, string user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.InsertExchangeRate", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bankId", bankId);
                cmd.Parameters.AddWithValue("@currencyInId", currencyInId);
                cmd.Parameters.AddWithValue("@currencyOutId", currencyOutId);
                cmd.Parameters.AddWithValue("@sell", selling);
                cmd.Parameters.AddWithValue("@purch", purch);
                if (percent != 0)
                {
                    cmd.Parameters.AddWithValue("@percent", percent);
                }
                cmd.Parameters.AddWithValue("@user", user);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public List<ExchangeModel> GetLogExchangeRate(DateTime? from, DateTime? to)
        {
            List<ExchangeModel> exchanges = new List<ExchangeModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.GetLogExchangeRates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                SqlDataReader reader = cmd.ExecuteReader();

                               List<string> logs = new List<string>();
                while (reader.Read())
                {
                    string log = reader["Log"] != DBNull.Value ? Convert.ToString(reader["Log"]) : string.Empty;
                    logs.Add(log);
                }
                List<BankModel> banks = GetBanks();
                List<CurrencyModel> currencies = GetCurrencies();
                foreach (string log in logs)
                {
                    List<ExchangeModel> exchange = JsonConvert.DeserializeObject<List<ExchangeModel>>(log);
                    dynamic temp = JsonConvert.DeserializeObject<dynamic>(log);
                    bool withPercent = Convert.ToBoolean(temp[0].b[0].WithPercent);
                    foreach (var item in exchange)
                    {
                        item.BankName = banks.Where(b => b.Id == item.BankId).FirstOrDefault().Name;
                        item.CurrencyInISO = currencies.Where(c => c.Id == item.CurrencyInId).FirstOrDefault().ISO;
                        item.CurrencyOutISO = currencies.Where(c => c.Id ==item.CurrencyOutId).FirstOrDefault().ISO;
                        item.WithPercent = withPercent;
                        exchanges.Add(item);
                    }
                }
            }
            return exchanges.Where(e => e.UpdateDate >= from).OrderBy(e => e.BankId).ThenBy(e => e.CurrencyInId).ThenBy(e => e.CurrencyOutId).ThenBy(e => e.UpdateDate).ThenBy(e => e.WithPercent).ToList();
            
        }
    }
}

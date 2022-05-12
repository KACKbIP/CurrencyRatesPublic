using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using CurrencyRates.Interfaces;
using CurrencyRates.Models;
using CurrencyRates.Models.Api;
using CurrencyRates.Models.ExchangeRate;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static CurrencyRates.Models.Api.CurrencyRateModel;

namespace CurrencyRates.Repositories
{
    public class ApiRepository : IApiRepository
    {
        private readonly string _connectionString;
        private readonly string _euBank;
        private readonly string _alfaBank;
        private readonly string _kaspiBank;
        private readonly string _halykBank;
        private readonly string _nationalBank;
        private readonly string _optimaBank;
        private readonly string _dosCredoBank;
        private readonly string _dosCredoBankNal;
        private readonly string _CBU;
        private readonly string _FMFB;
        private readonly string _alifBank;
        private readonly string _nationalBankTjs;
        private readonly string _nationalBankMDL;
        private readonly string _voltonBank;
        private readonly string _runpayBank;
        private readonly string _payvandBank;
        public ApiRepository(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainConnection");
            _euBank = configuration["Bank:Eurasian Bank"];
            _alfaBank = configuration["Bank:Alfa Bank"];
            _kaspiBank = configuration["Bank:Kaspi Bank"];
            _halykBank = configuration["Bank:Halyk Bank"];
            _nationalBank = configuration["Bank:NationalBank"];
            _optimaBank = configuration["Bank:Optima Bank"];
            _dosCredoBank = configuration["Bank:Dos CredoBank"];
            _dosCredoBankNal = configuration["Bank:Dos CredoBankNal"];
            _CBU = configuration["Bank:CBU"];
            _FMFB = configuration["Bank:FMFB"];
            _alifBank = configuration["Bank:Alif Bank"];
            _nationalBankTjs = configuration["Bank:NationalBankTJS"];
            _nationalBankMDL = configuration["Bank:NationalBankMDL"];
            _voltonBank = configuration["Bank:VoltonBank"];
            _runpayBank = configuration["Bank:RunPay Bank"];
            _payvandBank = configuration["Bank:Payvand Bank"];
        }        
        public List<CurrencyRateModel> GetCurrencyRatesForProcessing()
        {
            List<CurrencyRateModel> models = new List<CurrencyRateModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.GetCurrencyRates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CurrencyRateModel model = new CurrencyRateModel();
                    Rate rate = new Rate();
                    model.BankId            = reader["ProcessingId"] != DBNull.Value ? Convert.ToInt32(reader["ProcessingId"]) : 0;
                    rate.CurrencyInISO     = reader["CurrencyInISO"] != DBNull.Value ? Convert.ToString(reader["CurrencyInISO"]) : string.Empty;
                    rate.CurrencyOutISO    = reader["CurrencyOutISO"] != DBNull.Value ? Convert.ToString(reader["CurrencyOutISO"]) : string.Empty;
                    rate.SellingRate       = reader["SellingRate"] != DBNull.Value ? Convert.ToDouble(reader["SellingRate"]) : 0;
                    rate.PurchanseRate     = reader["PurchanseRate"] != DBNull.Value ? Convert.ToDouble(reader["PurchanseRate"]) : 0;
                    
                    if(models.Where(m=>m.BankId==model.BankId).Count()>0)
                    {
                        models.Where(m => m.BankId == model.BankId).FirstOrDefault().Rates.Add(rate);
                    }
                    else
                    {
                        model.Rates = new List<Rate>();
                        model.Rates.Add(rate);
                        models.Add(model);
                    }
                }
            }
            return models;
        }
        public void InsertCurriency(DataTable table, int bankId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.AutoUpdateExchangeRate", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@table", table);
                cmd.Parameters.AddWithValue("@bankId", bankId);
                SqlDataReader reader = cmd.ExecuteReader();
            }
        }
        public void SetNoAutoUpdate(int bankId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.SetNoAutoUpdate", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bankId", bankId);
                SqlDataReader reader = cmd.ExecuteReader();
            }
        }
       
        public static async Task SendTelegramMessageAsync(string message)
        {
                try
                {
                    WebRequest webRequest = WebRequest.Create("https://api.telegram.org/bot929218333:AAEVPi4-zsFXoHvPl_7Au155mj6-XvDniws/sendMessage");
                    webRequest.Method = "POST";

                    string postData = $"chat_id=@hggprocessingbotchannel&text={message}";

                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);

                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.ContentLength = byteArray.Length;

                    System.IO.Stream dataStream = webRequest.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse response = await webRequest.GetResponseAsync();
                    //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                    using (dataStream = response.GetResponseStream())
                    {
                        var reader = new System.IO.StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        //Console.WriteLine(responseFromServer);
                    }
                    response.Close();
                }
                catch (Exception e)
                {
                }
        }
        public string CrossCourse()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("dbo.CrossCourse", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return "Ok";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }
        public string EuBankCurrency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_euBank).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result;

                    var c = document.GetElementsByClassName("exchanges-tabs-list__item")[2].GetElementsByClassName("exchange-table__value");
                    var b = c;
                    //dynamic rates = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    table.Rows.Add("USD", "KZT", Convert.ToDouble(c[0].InnerHtml.Replace(".", ",")), Convert.ToDouble(c[4].InnerHtml.Replace(".", ",")));
                    table.Rows.Add("WMZ", "KZT", Convert.ToDouble(c[0].InnerHtml.Replace(".", ",")), Convert.ToDouble(c[4].InnerHtml.Replace(".", ",")));
                    table.Rows.Add("EUR", "KZT", Convert.ToDouble(c[1].InnerHtml.Replace(".", ",")), Convert.ToDouble(c[5].InnerHtml.Replace(".", ",")));
                    table.Rows.Add("RUB", "KZT", Convert.ToDouble(c[2].InnerHtml.Replace(".", ",")), Convert.ToDouble(c[6].InnerHtml.Replace(".", ",")));

                }
                InsertCurriency(table, 1);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(1);
                SendTelegramMessageAsync("Eurasian Bank: " + e.Message);
                return e.Message;
            }
        }
        public string AlfaCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_alfaBank).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    dynamic rates = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    table.Rows.Add("USD", "KZT", Convert.ToDouble(rates.items.USD.sell.value), Convert.ToDouble(rates.items.USD.buy.value));
                    table.Rows.Add("WMZ", "KZT", Convert.ToDouble(rates.items.USD.sell.value), Convert.ToDouble(rates.items.USD.buy.value));                
                    table.Rows.Add("EUR", "KZT", Convert.ToDouble(rates.items.EUR.sell.value), Convert.ToDouble(rates.items.EUR.buy.value));                
                    table.Rows.Add("RUB", "KZT", Convert.ToDouble(rates.items.RUB.sell.value), Convert.ToDouble(rates.items.RUB.buy.value));
                    
                }
                InsertCurriency(table, 1);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(1);
                SendTelegramMessageAsync("Alfa Bank: " + e.Message);
                return e.Message;
            }
        }
        public string KaspiCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));            
            try
            {

                #region bai.kz
               using (var httpClient = new HttpClient())
               {
                   var httpResponse =
                       httpClient.GetAsync(_kaspiBank).Result;
                   string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                   var context = BrowsingContext.New();
                   var document = context.OpenAsync(m => m.Content(responseContent)).Result;

                   //IElement item = document.Children[0].Children[1].Children[5].Children[1].Children[0].Children[3].Children[2].Children[0];
                   IElement item = document.Children[0].Children[1].Children[1].Children[0].Children[1].Children[0].Children[1].Children[1].Children[6].Children[1].Children[0].Children[1];
                   if (item.Children[1].Children[2].InnerHtml == "Доллар")
                   {
                       table.Rows.Add("USD", "KZT", Convert.ToDouble(item.Children[1].Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(item.Children[1].Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                       table.Rows.Add("WMZ", "KZT", Convert.ToDouble(item.Children[1].Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(item.Children[1].Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                   }
                   if (item.Children[2].Children[2].InnerHtml == "Евро")
                   {
                       table.Rows.Add("EUR", "KZT", Convert.ToDouble(item.Children[2].Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(item.Children[2].Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                   }
                   if (item.Children[3].Children[2].InnerHtml == "Российский рубль")
                   {
                       table.Rows.Add("RUB", "KZT", Convert.ToDouble(item.Children[3].Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(item.Children[3].Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                   }
                }
                #endregion
                #region kursi.kz
                //using (var httpClient = new HttpClient())
                //{
                //    var httpResponse =
                //        httpClient.GetAsync(_kaspiBank).Result;
                //    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                //    var context = BrowsingContext.New();
                //    var document = context.OpenAsync(m => m.Content(responseContent)).Result;
                //    var childs = document.All.Where(d => d.ClassName == "kyrsvalyt").First().Children[0];
                //    var usd = childs.Children[1];
                //    var eur = childs.Children[2];
                //    var rub = childs.Children[3];


                //    if (Convert.ToDouble(usd.Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")) == 0 ||
                //        Convert.ToDouble(eur.Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")) == 0 ||
                //        Convert.ToDouble(rub.Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")) == 0 ||
                //        Convert.ToDouble(usd.Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")) == 0 ||
                //        Convert.ToDouble(eur.Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")) == 0 ||
                //        Convert.ToDouble(rub.Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")) == 0)
                //        throw new Exception("Курс равен нулю, переключитесь в ручной режим");

                //table.Rows.Add("USD", "KZT", Convert.ToDouble(usd.Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(usd.Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                
                //table.Rows.Add("EUR", "KZT", Convert.ToDouble(eur.Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(eur.Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                
                //table.Rows.Add("RUB", "KZT", Convert.ToDouble(rub.Children[3].InnerHtml.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(rub.Children[4].InnerHtml.Replace(" ", "").Replace(".", ",")));
                //}
                #endregion
                InsertCurriency(table, 13);

                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(13);
                SendTelegramMessageAsync("Kaspi Bank: "+e.Message);
                return e.Message;
            }
        }
        public string HalykCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            List<BankCurrencyModel> list = new List<BankCurrencyModel>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_halykBank).Result;
                    var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    dynamic model="";
                    try
                    {
                        foreach (var item in JsonConvert.DeserializeObject<dynamic>(responseContent).data.currencyHistory)
                        {
                            foreach (var item2 in item)
                            {
                                foreach (var item3 in item2)
                                    if (item3.Name == "legalPersons")
                                    {
                                        model = item3.Value;
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                    catch
                    {
                        foreach (var item in JsonConvert.DeserializeObject<dynamic>(responseContent).data.currencyHistory)
                        {
                            foreach (var item2 in item)
                            {
                                    if (item2.Name == "legalPersons")
                                    {
                                        model = item2.Value;
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                    foreach (var item in model)
                    {
                        foreach (var item2 in item)
                        {
                            BankCurrencyModel halykModel = new BankCurrencyModel();
                            halykModel.CurrencyName = item.Name;
                            foreach (var item3 in item2)
                            {
                                if (item3.Name == "buy")
                                {
                                    halykModel.Selling = item3.Value;
                                }
                                if (item3.Name == "sell")
                                {
                                    halykModel.Purchanse = item3.Value;
                                }                                
                            }
                            list.Add(halykModel);
                            break;
                        }
                    };
                }
                foreach(var item in list)
                {
                    string[] curr = item.CurrencyName.Split('/');
                        table.Rows.Add(curr[0], curr[1], Convert.ToDouble(item.Selling.ToString("0.0000")), Convert.ToDouble(item.Purchanse.ToString("0.0000")));
                    if(curr[0]=="USD")
                    {
                        table.Rows.Add("WMZ", curr[1], Convert.ToDouble(item.Selling.ToString("0.0000")), Convert.ToDouble(item.Purchanse.ToString("0.0000")));
                    }
                }
                InsertCurriency(table, 2);

                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(2);
                SendTelegramMessageAsync("Halyk Bank: " + e.Message);
                return e.Message;
            }
        }
        public string NationalBankCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            List<NationalBank> models = new List<NationalBank>();
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(_nationalBank);
                XmlElement xRoot = xDoc.DocumentElement;
                foreach (XmlNode xnode in xRoot)
                    foreach(XmlNode item in xnode.ChildNodes)
                        if(item.LocalName== "item")
                        {
                            NationalBank model = new NationalBank();
                            foreach (XmlNode item2 in item.ChildNodes)
                            {
                                if (item2.LocalName == "title")
                                    model.ISO = item2.InnerText;
                                if (item2.LocalName == "description")
                                    model.Value = Convert.ToDouble(item2.InnerText.Replace(".",","));
                                if (item2.LocalName == "quant")
                                    model.Quantity = Convert.ToInt32(item2.InnerText);
                            }
                            models.Add(model);
                        }

                foreach(var item in models)
                    if(item.Quantity==1)
                        if(item.Value>1)
                            table.Rows.Add(item.ISO, "KZT", item.Value, item.Value);
                        else
                            table.Rows.Add("KZT", item.ISO, (1 / item.Value).ToString("0.00"), (1 / item.Value).ToString("0.00"));
                    else
                        if(item.Value/item.Quantity>1)
                            table.Rows.Add(item.ISO, "KZT", (item.Value/item.Quantity).ToString("0.00"), (item.Value/item.Quantity).ToString("0.00"));
                        else
                            table.Rows.Add("KZT", item.ISO, (item.Value / item.Quantity).ToString("0.00"), (item.Value / item.Quantity).ToString("0.00"));

                InsertCurriency(table, 3);
                return "Ok";
            }
            catch(Exception e)
            {
                SetNoAutoUpdate(3);
                SendTelegramMessageAsync("Национальный банк: " + e.Message);
                return e.Message;
            }
        }
        public string DosCredoCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_dosCredoBank).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result;
                    foreach (var item in document.Body.Children)
                    {
                        string curr = "";
                        double sell = 0;
                        double purch = 0;
                        string temp = item.TextContent.Replace("\\n        ", "").Replace("  ", " ");
                        string[] cur = temp.Split(' ');
                        curr = cur[0];
                        if (curr == "KZT")
                        {
                            purch = 1 / Convert.ToDouble(cur[1].Replace(".", ","));
                            sell = 1 / Convert.ToDouble(cur[2].Replace("\\n", "").Replace(".", ","));
                            if (sell > 1 && purch > 1)
                                table.Rows.Add("KGS", curr, Convert.ToDouble(sell.ToString("0.0000").Replace(".", ",")), Convert.ToDouble(purch.ToString("0.0000").Replace(".", ",")));
                        }
                        else
                        {
                            sell = Convert.ToDouble(cur[1].Replace(".", ","));
                            purch = Convert.ToDouble(cur[2].Replace("\\n", "").Replace(".", ","));
                            if (purch > sell)
                            {
                                table.Rows.Add(curr, "KGS", sell, purch);
                                if (curr == "USD")
                                {
                                    table.Rows.Add("WMZ", "KGS", sell, purch);
                                }
                            }
                        }
                    }
                }
                InsertCurriency(table, 4);
                InsertCurriency(table, 14);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(4);
                //SendTelegramMessageAsync("Дос - Кредобанк: " + e.Message);
                return e.Message;
            }
        }
        public string OptimaBank()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_optimaBank).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result.GetElementsByClassName("panel");
                    foreach(var item in document)
                    {
                        if(item.InnerHtml.Contains("Безналичные курсы"))
                        {
                            var item2 = item.GetElementsByClassName("row1");
                            foreach(var item3 in item2)
                            {
                                if(item3.InnerHtml.Contains("EUR"))
                                {
                                    var item4 = item3.GetElementsByClassName("up");
                                    table.Rows.Add("EUR", "KGS", Convert.ToDouble(item4[0].InnerHtml.Replace(".", ",")), Convert.ToDouble(item4[1].InnerHtml.Replace(".", ",")));
                                }
                                if (item3.InnerHtml.Contains("KZT"))
                                {
                                    var item4 = item3.GetElementsByClassName("up");
                                    table.Rows.Add("KGS", "KZT", 1/Convert.ToDouble(item4[1].InnerHtml.Replace(".", ",")), 1/Convert.ToDouble(item4[0].InnerHtml.Replace(".", ",")));
                                }
                            }
                            item2 = item.GetElementsByClassName("row0");
                            foreach (var item3 in item2)
                            {
                                if (item3.InnerHtml.Contains("USD"))
                                {
                                    var item4 = item3.GetElementsByClassName("up");
                                    table.Rows.Add("USD", "KGS", Convert.ToDouble(item4[0].InnerHtml.Replace(".", ",")), Convert.ToDouble(item4[1].InnerHtml.Replace(".", ",")));
                                }
                                if (item3.InnerHtml.Contains("RUB"))
                                {
                                    var item4 = item3.GetElementsByClassName("up");
                                    table.Rows.Add("RUB", "KGS", Convert.ToDouble(item4[0].InnerHtml.Replace(".", ",")), Convert.ToDouble(item4[1].InnerHtml.Replace(".", ",")));
                                }
                            }
                        }
                    }
                }
                InsertCurriency(table, 4);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(4);
                SendTelegramMessageAsync("Optima Bank: " + e.Message);
                return e.Message;
            }
        }
        public string DosCredoCurriencyNal()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_dosCredoBankNal).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result;
                    foreach (var item in document.Body.Children)
                    {
                        string curr = "";
                        double sell = 0;
                        double purch = 0;
                        string temp = item.TextContent.Replace("\\n        ", "").Replace("  ", " ");
                        string[] cur = temp.Split(' ');
                        curr = cur[0];
                        if (curr == "KZT")
                        {
                            purch = 1 / Convert.ToDouble(cur[1].Replace(".", ","));
                            sell = 1 / Convert.ToDouble(cur[2].Replace("\\n", "").Replace(".", ","));
                            if (sell > 1 && purch > 1)
                                table.Rows.Add("KGS", curr, Convert.ToDouble(sell.ToString("0.0000").Replace(".", ",")), Convert.ToDouble(purch.ToString("0.0000").Replace(".", ",")));
                        }
                        else
                        {
                            sell = Convert.ToDouble(cur[1].Replace(".", ","));
                            purch = Convert.ToDouble(cur[2].Replace("\\n", "").Replace(".", ","));
                            if (sell > 1 && purch > 1)
                            {
                                table.Rows.Add(curr, "KGS", sell, purch);
                                if (curr == "USD")
                                {
                                    table.Rows.Add("WMZ", "KGS", sell, purch);
                                }
                            }
                        }
                    }
                }
                InsertCurriency(table, 10);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(4);
                //SendTelegramMessageAsync("Дос - Кредобанк Наличные: " + e.Message);
                return e.Message;
            }
        }
        public string CBUCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            List<BankCurrencyModel> list = new List<BankCurrencyModel>();
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(_CBU);
                XmlElement xRoot = xDoc.DocumentElement;
                foreach (XmlNode xnode in xRoot)
                {
                    string curr="";
                    double sell=0;
                    foreach (XmlNode item in xnode.ChildNodes)
                    {
                        if (item.LocalName == "Ccy")
                        {
                            curr = item.InnerText;
                        }
                        if(item.LocalName == "Rate")
                        {
                            sell = Convert.ToDouble(item.InnerText.Replace(".",","));
                        }
                    };
                    if(sell<1)
                    {
                        table.Rows.Add("UZS", curr, (1 / sell).ToString("0.00"), (1 / sell).ToString("0.00"));
                    }
                    else
                    {
                        table.Rows.Add(curr, "UZS", sell.ToString("0.00"), sell.ToString("0.00"));
                    }
                }
                InsertCurriency(table, 5);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(5);
                //SendTelegramMessageAsync("The Central Bank of the Republic of Uzbekistan: " + e.Message);
                return e.Message;
            }

        }
        public string FMFMCurriency()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                double sellUSD = 0;
                double purchUSD = 0;
                double sellRUB = 0;
                double purchRUB = 0;
                double sellEUR = 0;
                double purchEUR = 0;
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_FMFB).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result;
                    var divSell = document.QuerySelectorAll("div.new-currency").Last().Children[0].Children[0].Children[0];
                    foreach (var item in divSell.Children)
                    {
                        if(item.TagName== "DIV")
                        {
                            foreach(var item2 in item.Children)
                            {
                                if(item2.ClassName== "flag flag-us")
                                {
                                    sellUSD = Convert.ToDouble(item.TextContent.Replace(".", ",").Replace(" ", ""));
                                }
                                if (item2.ClassName == "flag flag-eu")
                                {
                                    sellEUR = Convert.ToDouble(item.TextContent.Replace(".", ",").Replace(" ", ""));
                                }
                                if (item2.ClassName == "flag flag-ru")
                                {
                                    sellRUB = Convert.ToDouble(item.TextContent.Replace(".", ",").Replace(" ", ""));
                                }
                            }
                        }
                    }
                    var divPurch = document.QuerySelectorAll("div.new-currency").Last().Children[0].Children[0].Children[1];
                    foreach (var item in divPurch.Children)
                    {
                        if (item.TagName == "DIV")
                        {
                            foreach (var item2 in item.Children)
                            {
                                if (item2.ClassName == "flag flag-us")
                                {
                                    purchUSD = Convert.ToDouble(item.TextContent.Replace(".", ",").Replace(" ", ""));
                                }
                                if (item2.ClassName == "flag flag-eu")
                                {
                                    purchEUR = Convert.ToDouble(item.TextContent.Replace(".", ",").Replace(" ", ""));
                                }
                                if (item2.ClassName == "flag flag-ru")
                                {
                                    purchRUB = Convert.ToDouble(item.TextContent.Replace(".", ",").Replace(" ", ""));
                                }
                            }
                        }
                    }
                }
                table.Rows.Add("USD", "TJS", sellUSD, purchUSD);
                table.Rows.Add("RUB", "TJS", sellRUB, purchRUB);
                table.Rows.Add("EUR", "TJS", sellEUR, purchEUR);

                InsertCurriency(table, 6);
                return "Ok";
            }
            catch(Exception e)
            {
                SetNoAutoUpdate(6);
                //SendTelegramMessageAsync("Первый Микрофинансовый Банк: " + e.Message);
                return e.Message;
            }
        }
        public string AlifBank()        
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                            httpClient.GetAsync(_alifBank + $"?TransactionID={DateTime.Now.Ticks.ToString()}&RequestDate={DateTime.Now.ToString()}&Service=card_all&Account=5058270280412452&Currency=RUB&Amount=10").Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    var b = Convert.ToDecimal(JsonConvert.DeserializeObject <dynamic>(response.ResponseLog.Value).fx.Value.Replace(".",","));
                    table.Rows.Add("TJS", "RUB", (1/b).ToString("0.0000"), (1 / b).ToString("0.0000"));
                }
                InsertCurriency(table, 7);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(7);
                SendTelegramMessageAsync("Алиф Банк: " + e.Message);
                return e.Message;
            }
        }
        public string NationalBankTJS()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                string bigCur = "";
                string smallCur = "";
                double rate;
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_nationalBankTjs).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result;

                    //IElement item = document.Children[0].Children[1].Children[5].Children[1].Children[0].Children[3].Children[2].Children[0];
                    IElement item = document.Children[0].Children[1].Children[2].Children[3].Children[0].Children[3].Children[1].Children[0].Children[0];
                    foreach (var element in item.Children)
                    {
                        foreach(var item2 in element.Children)
                        {
                            if(item2.ClassName == "k_fColl")
                            {
                                bigCur = item2.InnerHtml.Split(' ')[1];
                                smallCur = "TJS";
                            }
                            if(item2.ClassName == "k_sColl")
                            {
                                rate = Convert.ToDouble(item2.InnerHtml.Replace('.', ','));
                                if(rate>1)
                                {
                                    table.Rows.Add(bigCur, smallCur, Convert.ToDouble(rate.ToString("0.0000").Replace(".", ",")), Convert.ToDouble(rate.ToString("0.0000").Replace(".", ",")));
                                }
                                else
                                {
                                    rate = 1 / rate;
                                    table.Rows.Add(smallCur, bigCur, Convert.ToDouble(rate.ToString("0.0000").Replace(".", ",")), Convert.ToDouble(rate.ToString("0.0000").Replace(".", ",")));
                                }
                            }
                        }
                    }
                }

                InsertCurriency(table, 9);

                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(9);
                //SendTelegramMessageAsync("Алиф Банк: " + e.Message);
                return e.Message;
            }
        }
        public string NationalBankMDL()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            List<NationalBank> models = new List<NationalBank>();
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(_nationalBankMDL+DateTime.Today.ToString("dd.MM.yyyy"));
                XmlElement xRoot = xDoc.DocumentElement;
                foreach (XmlNode xnode in xRoot)
                {
                    NationalBank model = new NationalBank();
                    foreach (XmlNode item2 in xnode)
                    {
                            if (item2.LocalName == "CharCode")
                                model.ISO = item2.InnerText;
                            if (item2.LocalName == "Value")
                                model.Value = Convert.ToDouble(item2.InnerText.Replace(".", ","));
                            if (item2.LocalName == "Nominal")
                                model.Quantity = Convert.ToInt32(item2.InnerText);
                    }
                    models.Add(model);
                }
                foreach (var item in models)
                    if (item.Quantity == 1)
                        if (item.Value > 1)
                            table.Rows.Add(item.ISO, "MDL", item.Value, item.Value);
                        else
                            table.Rows.Add("MDL", item.ISO, (1 / item.Value).ToString("0.00"), (1 / item.Value).ToString("0.00"));
                    else
                        if (item.Value / item.Quantity > 1)
                        table.Rows.Add(item.ISO, "MDL", (item.Value / item.Quantity).ToString("0.00"), (item.Value / item.Quantity).ToString("0.00"));
                    else
                        table.Rows.Add("MDL", item.ISO, (1 / item.Value / item.Quantity).ToString("0.00"), (1 / item.Value / item.Quantity).ToString("0.00"));

                InsertCurriency(table, 11);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(11);
                SendTelegramMessageAsync("Национальный банк Молдовы: " + e.Message);
                return e.Message;
            }
        }
        public string VoltonBank()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(_voltonBank+ "4378");
                XmlElement xRoot = xDoc.DocumentElement;
                foreach (XmlAttribute item in xRoot.FirstChild.Attributes)
                {                    
                    if(item.Name== "service-rate")
                    {
                        table.Rows.Add("USD", "RUB", Convert.ToDouble(item.Value.Replace('.',',')), Convert.ToDouble(item.Value.Replace('.', ',')));
                    }
                }
                xDoc = new XmlDocument();
                xDoc.Load(_voltonBank + "4407");
                xRoot = xDoc.DocumentElement;
                foreach (XmlAttribute item in xRoot.FirstChild.Attributes)
                {
                    if (item.Name == "service-rate")
                    {
                        table.Rows.Add("EUR", "RUB", Convert.ToDouble(item.Value.Replace('.', ',')), Convert.ToDouble(item.Value.Replace('.', ',')));
                    }
                }
                xDoc = new XmlDocument();
                xDoc.Load(_voltonBank + "4380");
                xRoot = xDoc.DocumentElement;
                foreach (XmlAttribute item in xRoot.FirstChild.Attributes)
                {
                    if (item.Name == "service-rate")
                    {
                        table.Rows.Add("UAH", "RUB", Convert.ToDouble(item.Value.Replace('.', ',')), Convert.ToDouble(item.Value.Replace('.', ',')));
                    }
                }

                InsertCurriency(table, 12);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(12);
                SendTelegramMessageAsync("Волтон Банк: " + e.Message);
                return e.Message;
            }
        }
        public string AgroindbankMDL()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_runpayBank).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    var context = BrowsingContext.New();
                    var document = context.OpenAsync(m => m.Content(responseContent)).Result;

                   var item = document.GetElementsByClassName("table table-condensed table-borderless table-striped").FirstOrDefault().Children[1];
                    if(item.Children[0].Children[1].InnerHtml== "<strong>EUR</strong>")
                    {
                        table.Rows.Add("EUR", "MDL", Convert.ToDouble(item.Children[0].Children[2].InnerHtml.Replace(" MDL", "").Replace(".", ",")), Convert.ToDouble(item.Children[0].Children[3].InnerHtml.Replace(" MDL", "").Replace(".", ",")));
                    }
                    if (item.Children[1].Children[1].InnerHtml == "<strong>USD</strong>")
                    {
                        table.Rows.Add("USD", "MDL", Convert.ToDouble(item.Children[1].Children[2].InnerHtml.Replace(" MDL", "").Replace(".", ",")), Convert.ToDouble(item.Children[1].Children[3].InnerHtml.Replace(" MDL", "").Replace(".", ",")));
                    }
                }

                InsertCurriency(table, 15);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(15);
                SendTelegramMessageAsync("RunPay Bank: " + e.Message);
                return e.Message;
            }
        }

        public string PayvandTJS()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CurrencyInISO", typeof(string));
            table.Columns.Add("CurrencyOutISO", typeof(string));
            table.Columns.Add("SellingRate", typeof(double));
            table.Columns.Add("PurchanseRate", typeof(double));
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse =
                        httpClient.GetAsync(_payvandBank).Result;
                    string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    double let = 1;

                    table.Rows.Add("USD", "TJS", response.rates.data.usd.buy, response.rates.data.usd.sell);
                    table.Rows.Add("EUR", "TJS", response.rates.data.eur.buy, response.rates.data.eur.sell);

                    double rurSell = response.rates.data.rur.sell;
                    double rurBuy = response.rates.data.rur.buy;

                    double rSell = let / rurSell;
                    double rBuy = let / rurBuy;

                    table.Rows.Add("TJS", "RUB", rSell, rBuy);

                }

                InsertCurriency(table, 16);
                return "Ok";
            }
            catch (Exception e)
            {
                SetNoAutoUpdate(16);
                SendTelegramMessageAsync("Payvand Bank: " + e.Message);
                return e.Message;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Forex.Models;
using Newtonsoft.Json;

namespace Forex.Controllers
{
    public class ExchangeRateController : ApiController
    {
        // GET api/<controller>
        private const string forexApi = "forexApi";
        private const string contentType = "application/json; charset=utf-8";
        [System.Web.Http.AcceptVerbs("Get")]
        public string Ping()
        {
            return "OK";
        }

        public string GetExchangeRates(DateTime startDate, DateTime endDate, string baseCurrency, string targetCurrency)
        {
            string uri = ConfigurationManager.AppSettings[forexApi];

            string url = string.Format(@"{0}history?base={1}&start_at={2}&end_at={3}&symbols={4}",
                        uri, baseCurrency, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), targetCurrency);

            string resultString = string.Empty;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            WebResponse webResp = webRequest.GetResponse();

            Stream stream = webResp.GetResponseStream();
            StreamReader readStream = new StreamReader(stream, Encoding.UTF8);
            string result = readStream.ReadToEnd();

            // client class details not exposed, so dynamic type used heres
            dynamic exRates = JsonConvert.DeserializeObject<dynamic>(result);
            dynamic rates = JsonConvert.DeserializeObject<dynamic>(exRates["rates"].ToString());
            List<ExchangeRate> exchangeRates = new List<ExchangeRate>();
            ExchangeRate exRate = default(ExchangeRate);

            foreach (var rate in rates.Children())
            {
                exRate = new ExchangeRate();
                exRate.ExchangeDate = Convert.ToDateTime(rate.Name);
                foreach (var value in rate.Value.Children())
                {
                    exRate.Currency = value.Name;
                    exRate.ExchangeValue = value.Value;
                }

                exchangeRates.Add(exRate);
                exRate = default(ExchangeRate);
            }

            decimal minRate = exchangeRates.Min(e => e.ExchangeValue);
            decimal maxRate = exchangeRates.Max(e => e.ExchangeValue);
            decimal avgRate = exchangeRates.Average(e => e.ExchangeValue);

            DateTime minRateDate = exchangeRates.FirstOrDefault(e => e.ExchangeValue == minRate).ExchangeDate;
            DateTime maxRateDate = exchangeRates.FirstOrDefault(e => e.ExchangeValue == maxRate).ExchangeDate;

            var finalResult = new
            {
                MinRate = minRate,
                MaxRate = maxRate,
                AverageRate = avgRate,
                MinRateAt = minRateDate,
                MaxRateAt = maxRateDate
            };


            string finalResultString = JsonConvert.SerializeObject(finalResult);
            return finalResultString;

        }

    }
}
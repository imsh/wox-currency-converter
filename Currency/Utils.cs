using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Wox.Plugin;

namespace Currency
{
    class Utils
    {
        public decimal ParseAmount(string amount)
        {
            decimal result = 0;
            if (!Decimal.TryParse(amount, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                Decimal.TryParse(amount, NumberStyles.Any, new CultureInfo("en-US"), out result);
            }
            return result;
        }

        private decimal GetExchange(string fromCurrency, string toCurrency, decimal amount)
        {
            var apiKey = GetApiKey();
            var url = "https://free.currconv.com/api/v7/convert";
            var urlParameters = $"?q={fromCurrency}_{toCurrency}&compact=y&apiKey={apiKey}";
      
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var stringResponse = response.Content.ReadAsStringAsync().Result;
                    JObject res = JObject.Parse(stringResponse);
                    var result = res[$"{fromCurrency}_{toCurrency}"]["val"].Value<decimal>();
                    Console.WriteLine("{0} ({1}) = {2}", (int)response.StatusCode, response.ReasonPhrase, result);

                    return result * amount;
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);

                    return -1;
                }
            }
        }

        public List<Result> GetResult(SearchParams sp)
        {
            var results = new List<Result>();

            var exchange = GetExchange(sp.FromCurrency, sp.ToCurrency, sp.Amount);

            if (exchange > 0)
            {
                results.Add(new Result
                {
                    Title = $"{sp.Amount:N} {sp.FromCurrency} = {exchange:N} {sp.ToCurrency}",
                    IcoPath = "Images/icon.png",
                    SubTitle = $"Source: https://free.currencyconverterapi.com"
                });
            }
            else
            {
                results.Add(new Result
                {
                    Title = $"Somethings went wrong...",
                    IcoPath = "Images/icon.png"
                });
            }
            //Debug: Add {APIDebug(sp.FromCurrency, sp.ToCurrency)} to Result.Title
            return results;
        }

        public List<Result> GetMessage(string message)
        {
            var results = new List<Result>();

            results.Add(new Result
            {
                Title = $"Error: {message}",
                IcoPath = "Images/icon.png"
            });

            return results;
        }

        public List<Result> GetMessage(string message, string desc)
        {
            var results = new List<Result>();

            results.Add(new Result
            {
                Title = $"Error: {message}",
                SubTitle = $"{desc}",
                IcoPath = "Images/icon.png"
            });

            return results;
        }

        private string GetApiKey()
        {
            return Properties.Settings.Default.apiKey;  
        }
    }
}

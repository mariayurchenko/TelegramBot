using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TelegramBot
{
    public static class GetCurrency
    {
        public static Currency Currency(string date)
        {
            if (string.IsNullOrEmpty(date))
                throw new ArgumentNullException();

            string url = "https://api.privatbank.ua/p24api/exchange_rates?json&date=" + date;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string responce;
            using (StreamReader str = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                responce = str.ReadToEnd();
            }
            Currency currency = JsonConvert.DeserializeObject<Currency>(responce);

            return currency;
        }
    }
}

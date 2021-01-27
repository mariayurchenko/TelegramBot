using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot
{
    public class Currency
    {
        public DateTime Date { get; set; }
        public string BaseCurrencyLit { get; set; }
        public List<ExchangeRate> ExchangeRate { get; set; }
    }
}

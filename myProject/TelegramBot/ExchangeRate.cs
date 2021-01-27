using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot
{
    public class ExchangeRate
    {
        public string Currency { get; set; }
        public double SaleRate { get; set; }
        public double PurchaseRate { get; set; }
    }
}

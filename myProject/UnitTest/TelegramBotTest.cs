using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelegramBot;

namespace UnitTest
{
    [TestClass]
    public class TelegramBotTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenEnterEmptyStringOnGetCurrencyThrowArgumentException()
        {
            GetCurrency.Currency("");
        }
        [DataTestMethod]
        [DataRow("11.10.2020", "CZK", 1.2400000, 1.0300000)]
        [DataRow("12.06.2019", "EUR", 30.1000000, 29.2500000)]
        [DataRow("01.01.2016", "USD", 25.7000000, 25.2000000)]
        public void WhenEnterTheDateAndCurrencyGetCurrencyRate(string date, string currency, double saleRate,
            double purchaseRate)
        {
            foreach (var obj in GetCurrency.Currency(date).ExchangeRate)
            {
                if (obj.Currency == currency)
                {
                    Assert.AreEqual(obj.SaleRate, saleRate);
                    Assert.AreEqual(obj.PurchaseRate, purchaseRate);
                }
            }
        }
    }
}

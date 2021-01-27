using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Telegram.Bot;

namespace TelegramBot
{
    public class Bot
    {
        private ITelegramBotClient botClient;
        public Bot(string api)
        {
            botClient = new TelegramBotClient(api);
        }

        public void Start()
        {
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"I am user {me.Id} and my name is {me.FirstName}.");
            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
        }

        private async void BotClient_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data == "CHF" || e.CallbackQuery.Data == "RUR" ||
                e.CallbackQuery.Data == "EUR" || e.CallbackQuery.Data == "USD" ||
                e.CallbackQuery.Data == "GBP" || e.CallbackQuery.Data == "PLZ" ||
                e.CallbackQuery.Data == "SEK" || e.CallbackQuery.Data == "XAU" ||
                e.CallbackQuery.Data == "CAD")
            {
                currency = Convert.ToString(e.CallbackQuery.Data);
                if (dateStr != null)
                {
                    SelectCommand();
                }
                else
                {
                    await botClient.SendTextMessageAsync(Id,
                        "Введите /date чтобы указать дату.");
                }
            }
            else if (e.CallbackQuery.Data == "command1")
            {
                try
                {
                    var exchangeRate = GetExchangeRate();
                    await botClient.SendTextMessageAsync(Id,
                        $"Продажа: {exchangeRate.SaleRate} " +
                        $"Покупка: {exchangeRate.PurchaseRate}");
                }
                catch (Exception exception)
                {
                    await botClient.SendTextMessageAsync(Id,
                        exception.Message);
                }

            }
            else if (e.CallbackQuery.Data == "command2")
            {
                await botClient.SendTextMessageAsync(Id,
                    "Введите /date чтобы указать другую дату");
            }
            else if (e.CallbackQuery.Data == "command3")
            {
                await botClient.SendTextMessageAsync(Id,
                    "Введите /currency чтобы выбереть другой курс");
            }
            else
            {
                await botClient.SendTextMessageAsync(Id,
                    "Вы ввели неверную комманду!");
            }
        }

        private ExchangeRate GetExchangeRate()
        {
            foreach (var obj in GetCurrency.Currency(dateStr).ExchangeRate)
            {
                if (obj.Currency == currency)
                {
                    return obj;
                }
            }
            return new ExchangeRate();
        }

        private static string dateStr;
        private static string currency;

        private void CheckDate(string date)
        {
            var dateFormat = "dd.MM.yyyy";
            DateTime scheduleDate;

            if (DateTime.TryParseExact(date, dateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out scheduleDate))
            {
                if (scheduleDate <= DateTime.Today)
                {
                    dateStr = date;
                }
                else
                {
                    throw new ArgumentException("Вы ввели дату больше текущей!");
                }
            }
            else
            {
                throw new ArgumentException($"{date} не верный формат даты.");
            }

        }

        private long Id;

        private async void SelectCommand()
        {
            var keyb = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                new Telegram.Bot.Types.InlineKeyboardButton[][]
                {
                    new[]
                    {
                        new Telegram.Bot.Types.InlineKeyboardButton("Да, просмотреть курс",
                            "command1")
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.InlineKeyboardButton("Нет, выбрать другую дату",
                            "command2")
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.InlineKeyboardButton("Нет, выбрать другой курс",
                            "command3")
                    }
                }
            );
            await botClient.SendTextMessageAsync(Id,
                $"Вы действительно желаете просмотреть курс {currency} по дате {dateStr}?", false,
                false, 0, keyb, Telegram.Bot.Types.Enums.ParseMode.Default);
        }


        private async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message == null) return;
            Id = e.Message.Chat.Id;

            switch (e.Message.Text.Split(' ').First())
            {
                case "/start":
                    {

                        await botClient.SendTextMessageAsync(e.Message.Chat.Id,
                            @"Этот бот служит для возврата курса валюты на дату (по отношению к гривне) 
Доступные команды:
/start Посмотреть команды.
/currency Выбрать код валюты.
/date Указать дату.

                    ");

                        break;
                    }


                case "/date":
                    {
                        await botClient.SendTextMessageAsync(e.Message.Chat.Id, "Введите дату в формате дд.мм.гггг");

                        break;
                    }

                case "/currency":
                    {
                        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(
                            new Telegram.Bot.Types.InlineKeyboardButton[][]
                            {
                            new[]
                            {
                                new Telegram.Bot.Types.InlineKeyboardButton("USD", "USD"),
                                new Telegram.Bot.Types.InlineKeyboardButton("EUR", "EUR"),
                                new Telegram.Bot.Types.InlineKeyboardButton("RUR", "RUR"),
                                new Telegram.Bot.Types.InlineKeyboardButton("CHF", "CHF")
                            },
                            new[]
                            {
                                new Telegram.Bot.Types.InlineKeyboardButton("GBP", "GBP"),
                                new Telegram.Bot.Types.InlineKeyboardButton("PLZ", "PLZ"),
                                new Telegram.Bot.Types.InlineKeyboardButton("SEK", "SEK"),
                                new Telegram.Bot.Types.InlineKeyboardButton("XAU", "XAU"),
                                new Telegram.Bot.Types.InlineKeyboardButton("CAD", "CAD")
                            }
                            });
                        await botClient.SendTextMessageAsync(e.Message.Chat.Id, "Список доступных валют:", false, false, 0,
                            keyboard, Telegram.Bot.Types.Enums.ParseMode.Default);

                        break;
                    }

                default:
                    {
                        DateTime dt;

                        if (DateTime.TryParse(e.Message.Text, out dt))
                        {
                            try
                            {
                                CheckDate(e.Message.Text);
                                if (currency == null)
                                {
                                    await botClient.SendTextMessageAsync(e.Message.Chat.Id,
                                        $"Ваша дата: {e.Message.Text}. Введите /currency чтобы выбрать курс.");
                                }
                                else
                                {
                                    SelectCommand();
                                }
                            }
                            catch (Exception exception)
                            {
                                await botClient.SendTextMessageAsync(e.Message.Chat.Id,
                                    exception.Message);
                            }
                        }
                        else
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id,
                                $"Я тебя не понимаю. Введи /start чтобы просмотреть список доступных команд.");

                        break;
                    }
            }

        }
    }
}

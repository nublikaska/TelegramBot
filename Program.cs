using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.Echo
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("259629628:AAGW3exFto-l7uovTW3NXsr2fdoFWL1OWsU");

        static void Main(string[] args)
        {            
            Bot.OnMessage += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;

            if (message.Text.StartsWith("/AverageVolGazpromForToday")) // send inline keyboard
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);             
                await Task.Delay(500); // simulate longer running task
                Finam finam = new Finam();
                String r = "RadioButtonGazprom";
                await Bot.SendTextMessageAsync(message.Chat.Id, finam.GetTransactionForDay(DateTime.Now.ToShortDateString().ToString(), r, "Vol"));
            }
            else if (message.Text.StartsWith("/AverageVolSberForToday")) // send inline keyboard
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(500); // simulate longer running task
                Finam finam = new Finam();
                String r = "RadioButtonSberBank";
                await Bot.SendTextMessageAsync(message.Chat.Id, finam.GetTransactionForDay(DateTime.Now.ToShortDateString().ToString(), r, "Vol"));
            }
            else
            {
                var usage = @"Usage:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage,
                    replyMarkup: new ReplyKeyboardHide());
            }
        }
    }
}

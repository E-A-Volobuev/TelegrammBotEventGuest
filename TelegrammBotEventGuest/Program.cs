using LightInject;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegrammBotEventGuest.BLL.Repositories;
using TelegrammBotEventGuest.BLL.Services;
using TelegrammBotEventGuest.Core.Interfaces;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest
{
    class Program
    {
        private static ITelegramBotClient bot = new TelegramBotClient("5918699351:AAF6Gl4WRPlcbbc6XM8oOdnahcT-OdmKoWM");
        private static string idEvent = string.Empty;
        private List<EventEntity> listEvent = new List<EventEntity>();

        /// <summary>
        /// обработка сообщений пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                GetIdEvent(update);

                var container = new ServiceContainer();
                container.Register<IRequestService, RequestService>();
                var method = container.GetInstance<IRequestService>();

                await method.HandleUpdateAsync(botClient, update, cancellationToken, idEvent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        /// <summary>
        /// вывод ошибок в консоль , метод асинхронный, т.к. bot.StartReceiving принимает только асинхронные методы
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Task.Run(()=>Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception)));
        }


        async static Task Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            Program pr = new Program();
            pr.listEvent = await pr.GetAllEventsAsync();

            bot.StartReceiving(
            pr.HandleUpdateAsync,
            pr.HandleErrorAsync,
            receiverOptions,
            cancellationToken);


            while (true)
                await pr.RemindAsync();

        }

        private void GetIdEvent(Update update)
        {
            if (update.CallbackQuery?.Data.Contains("editNameCardEvents_") == true)
                idEvent = update.CallbackQuery.Data.Replace("editNameCardEvents_", "");
            if (update.CallbackQuery?.Data.Contains("editDescriptionCardEvents_") == true)
                idEvent = update.CallbackQuery.Data.Replace("editDescriptionCardEvents_", "");
            if (update.CallbackQuery?.Data.Contains("editDateCardEvents_") == true)
                idEvent = update.CallbackQuery.Data.Replace("editDateCardEvents_", "");
            if (update.CallbackQuery?.Data.Contains("editPhotoCardEvents_") == true)
                idEvent = update.CallbackQuery.Data.Replace("editPhotoCardEvents_", "");
            if (update.CallbackQuery?.Data.Contains("cancelCardEvents_") == true)
                idEvent = update.CallbackQuery.Data.Replace("cancelCardEvents_", "");
            if (update.CallbackQuery?.Data.Contains("registerForEvent_") == true)
                idEvent = update.CallbackQuery.Data.Replace("registerForEvent_", "");
            if (update.CallbackQuery?.Data.Contains("cancellRegister_") == true)
                idEvent = update.CallbackQuery.Data.Replace("cancellRegister_", "");

        }


        /// <summary>
        ///  напоминание пользователю о событии
        /// </summary>
        /// <returns></returns>
        private async Task RemindAsync()
        {
            try
            {
                var container = new ServiceContainer();
                container.Register<IRemindEventsService, RemindEventsService>();
                var method = container.GetInstance<IRemindEventsService>();

                await method.RemindEventAsync(bot,listEvent);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private async Task<List<EventEntity>> GetAllEventsAsync()
        {
            var container = new ServiceContainer();
            container.Register<IEventRepository, EventRepository>();
            var method = container.GetInstance<IEventRepository>();

            var list = await method.GetAllAsync();
            return list;
        }
    }
}


using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegrammBotEventGuest.BLL.Repositories;
using TelegrammBotEventGuest.Core.Interfaces;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.BLL.Services
{
    public class RemindEventsService : IRemindEventsService
    {
        /// <summary>
        /// напоминание пользователю о событии
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="listEvent"></param>
        /// <returns></returns>
        public async Task RemindEventAsync(ITelegramBotClient botClient, List<EventEntity> listEvent)
        {
            DateTime currentDate = DateTime.Now;
            DateTime twelveDate = currentDate.AddHours(12);
            DateTime twentyFourDate = currentDate.AddHours(24);

            /////если мероприятие уже началось
            string currentDateString = currentDate.ToString();
            var currentDateEvents = listEvent.Where(x => x.Date.ToString() == currentDateString).ToList();

            await DateRemindHelperAsync(botClient, currentDateEvents, 0);

            /////если мероприятие начнется через 12 часов
            string twelveDateString = twelveDate.ToString();
            var twelveDateEvents = listEvent.Where(x => x.Date.ToString() == twelveDateString).ToList();

            await DateRemindHelperAsync(botClient, twelveDateEvents, 12);

            /////если мероприятие начнется через 24 часа
            string twentyFourDateString = twentyFourDate.ToString();
            var twentyFourDateEvents = listEvent.Where(x => x.Date.ToString() == twentyFourDateString).ToList();

            await DateRemindHelperAsync(botClient, twentyFourDateEvents, 24);
        }

        /// <summary>
        /// проверка времени до начала мероприятия и вызов соответствующего уведомления
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="listEvent"></param>
        /// <returns></returns>
        private async Task DateRemindHelperAsync(ITelegramBotClient botClient, List<EventEntity> listEvent, int hourCount)
        {
            if (listEvent.Count > 0)
            {
                foreach (var currrentEvent in listEvent)
                {
                    await RemindSenderAsync(botClient, currrentEvent, hourCount);
                }
            }
        }

        /// <summary>
        /// в зависимости от количества времени до события создаем сообщение-напоминание
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="ev"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private async Task RemindSenderAsync(ITelegramBotClient botClient, EventEntity ev, int time)
        {
            var guests = await GetGuestsByEventAsync(ev);

            string text = @"⏰ <b>напоминание</b> ❗️❗️❗️" + "\n" +
                           "Вы регистрировались на событие: " + @$"<b>{ev.Name}</b>" + "\n";

            string textByTimePlanned = "Мероприятие начнется через " + @$"<b>{time}</b>";
            string textTimeNow = @"<b>Мероприятие началось</b>❗️❗️❗️";

            await RemindSenderHelperAsync(botClient, guests,  time, text, textByTimePlanned, textTimeNow);
        }
        private async Task RemindSenderHelperAsync(ITelegramBotClient botClient, List<GuestEntity> guests, int time, string text, string textByTimePlanned, string textTimeNow)
        {
            foreach (var guest in guests)
            {
                if (time == 0)
                    await botClient.SendTextMessageAsync(guest.ChatId, text + textTimeNow, ParseMode.Html);
                else if (time == 12)
                    await botClient.SendTextMessageAsync(guest.ChatId, text + textByTimePlanned + @" <b>часов</b> ❗️❗️❗️", ParseMode.Html);
                else
                    await botClient.SendTextMessageAsync(guest.ChatId, text + textByTimePlanned + @" <b>часа</b> ❗️❗️❗️", ParseMode.Html);
            }
        }

        /// <summary>
        /// список гостей события
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        private async Task<List<GuestEntity>> GetGuestsByEventAsync(EventEntity ev)
        {
            var container = new ServiceContainer();
            container.Register<IGuestEntityRepository, GuestEntityRepository>();
            var method = container.GetInstance<IGuestEntityRepository>();

            var guests = await method.GetAllGuestsByCurrentEventAsync(ev.Id);

            return guests;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.Core.Interfaces
{
    public interface IRemindEventsService
    {
        /// <summary>
        /// напоминание о событии
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="listEvent"></param>
        /// <returns></returns>
       Task RemindEventAsync(ITelegramBotClient botClient, List<EventEntity> listEvent);
    }
}

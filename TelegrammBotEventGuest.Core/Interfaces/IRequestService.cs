using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegrammBotEventGuest.Core.Interfaces
{
    public interface IRequestService
    {
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string idEvent);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.Core.Interfaces
{
    public interface IGuestEntityRepository : IBaseRepository<GuestEntity>
    {
        Task<GuestEntity> GetGuestByChatIdAsync(long chatId);
        Task<GuestEntity> GetGuestByEventAsync(long chatId, Guid entityId);
        Task<GuestEntity> GetAdminAsync(long chatId);
        Task<List<GuestEntity>> GetAllGuestsByChatIdAsync(long chatId);
        Task CancelRegistrationAsync(long chatId, Guid entityId);
        Task<List<GuestEntity>> GetAllGuestsByCurrentEventAsync(Guid entityId);
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegrammBotEventGuest.Core.Interfaces;
using TelegrammBotEventGuest.DataAccessLayer;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.BLL.Repositories
{
    public class GuestEntityRepository: BaseRepository<GuestEntity>, IGuestEntityRepository
    {
        public async Task<GuestEntity> GetByIdAsync(Guid id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var obj = await db.GuestEntity.FirstOrDefaultAsync(x => x.Id == id);
                return obj;
            }
        }

        /// <summary>
        /// получение пользователя по chatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<GuestEntity> GetGuestByChatIdAsync(long chatId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var obj = await db.GuestEntity.FirstOrDefaultAsync(x => x.ChatId == chatId);
                return obj;
            }
        }

        /// <summary>
        /// получение пользователя, зарегистрированного на событие
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public async Task<GuestEntity> GetGuestByEventAsync(long chatId, Guid entityId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var obj = await db.GuestEntity.FirstOrDefaultAsync(x => x.ChatId == chatId && x.EventEntityId == entityId);
                return obj;
            }
        }

        /// <summary>
        /// проверка на админа
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<GuestEntity> GetAdminAsync(long chatId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var obj = await db.GuestEntity.FirstOrDefaultAsync(x => x.ChatId == chatId && x.UserType==UserType.ADMIN);
                return obj;
            }
        }
        public async Task<List<GuestEntity>> GetAllGuestsByChatIdAsync(long chatId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var guests = await db.GuestEntity.Where(x => x.ChatId == chatId).ToListAsync();
                return guests;
            }
        }
        public async Task CancelRegistrationAsync(long chatId, Guid entityId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var guest = await db.GuestEntity.FirstOrDefaultAsync(x => x.ChatId == chatId && x.EventEntityId == entityId);

                db.GuestEntity.Remove(guest);
                await db.SaveChangesAsync();
            }
        }
        /// <summary>
        /// получение пользователей, зарегистрированных на событие
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public async Task<List<GuestEntity>> GetAllGuestsByCurrentEventAsync(Guid entityId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var guests = await db.GuestEntity.Where(x => x.EventEntityId == entityId).ToListAsync();
                return guests;
            }
        }
    }
}

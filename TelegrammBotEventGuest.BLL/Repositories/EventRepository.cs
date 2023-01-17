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
    public class EventRepository:BaseRepository<EventEntity>, IEventRepository
    { 
        public async Task<EventEntity> GetByIdAsync(Guid id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var obj = await db.EventEntity.FirstOrDefaultAsync(x=>x.Id==id);
                return obj;
            }
        }
        /// <summary>
        /// вывод событий определенного месяца
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<List<EventEntity>> GetEventsByMonthAsync(int month)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var eventsByMonth = await db.EventEntity.Where(x=>x.Date.Month==month).ToListAsync();
                return eventsByMonth;
            }
        }
    }
}

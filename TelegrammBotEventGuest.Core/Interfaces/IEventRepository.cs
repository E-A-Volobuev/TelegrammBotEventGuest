using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.Core.Interfaces
{
    public interface IEventRepository:IBaseRepository<EventEntity>
    {
        /// <summary>
        /// вывод событий определенного месяца
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<List<EventEntity>> GetEventsByMonthAsync(int month);

        Task<EventEntity> GetByIdAsync(Guid id);

    }
}

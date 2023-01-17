using System;

namespace TelegrammBotEventGuest.DAL.Entities
{
    /// <summary>
    /// сущность пользователя
    /// </summary>
    public class GuestEntity
    {
        public Guid Id { get; set; }
        /// <summary>
        /// id диалога с пользователем (уникальный идентификатор пользователя в телеграмм)
        /// </summary>
        public long ChatId { get; set; } 
        public Guid EventEntityId { get; set; }
        public EventEntity EventEntity { get; set; }
    }
}

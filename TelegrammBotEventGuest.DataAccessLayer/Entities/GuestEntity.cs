using System;

namespace TelegrammBotEventGuest.DataAccessLayer.Entities
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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserType UserType { get; set; }
        public Guid? EventEntityId { get; set; }
        public EventEntity EventEntity { get; set; }
    }

    public enum UserType
    {
        GUEST,
        ADMIN
    }
}

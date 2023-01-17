using System;
using System.Collections.Generic;

namespace TelegrammBotEventGuest.DataAccessLayer.Entities
{
    public class EventEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// дата проведения
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// фотографии
        /// </summary>
        public string PathPhoto { get; set; }

        /// <summary>
        /// гости
        /// </summary>
        public List<GuestEntity> GuestEntity { get; set; }

        public EventEntity()
        {
            GuestEntity = new List<GuestEntity>();
        }
    }
}

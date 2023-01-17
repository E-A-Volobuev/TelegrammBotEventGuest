using System;
using System.Collections.Generic;

namespace TelegrammBotEventGuest.DAL.Entities
{
    public class EventEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PathPhoto { get; set; }

        public List<GuestEntity> GuestEntity { get; set; }

        public EventEntity()
        {
            GuestEntity = new List<GuestEntity>();
        }
    }
}

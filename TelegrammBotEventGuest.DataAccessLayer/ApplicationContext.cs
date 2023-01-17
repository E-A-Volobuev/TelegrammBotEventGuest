using Microsoft.EntityFrameworkCore;
using System.IO;
using TelegrammBotEventGuest.DataAccessLayer.Entities;

namespace TelegrammBotEventGuest.DataAccessLayer
{
    public class ApplicationContext : DbContext
    {
        public DbSet<GuestEntity> GuestEntity { get; set; }
        public DbSet<EventEntity> EventEntity { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.Combine(Path.GetTempPath(), "botApp.db");
            if (!File.Exists(path))
                File.WriteAllBytes(path, new byte[0]);

            optionsBuilder.UseSqlite("Data Source="+ path);
        }
    }
}

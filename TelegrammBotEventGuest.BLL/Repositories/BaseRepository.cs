using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegrammBotEventGuest.DataAccessLayer;

namespace TelegrammBotEventGuest.BLL.Repositories
{
    public class BaseRepository<T> where T:class
    {
        public async Task<List<T>> GetAllAsync()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                DbSet<T> dbSet = db.Set<T>();

                var list = await dbSet.AsNoTracking().ToListAsync();
                return list;
            }
        }
        public async Task CreatAsync(T entity)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                DbSet<T> dbSet = db.Set<T>();

                dbSet.Add(entity);
                await db.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(T entity)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                DbSet<T> dbSet = db.Set<T>();

                dbSet.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                DbSet<T> dbSet = db.Set<T>();

                dbSet.Update(entity);
                await db.SaveChangesAsync();
            }
        }
    }
}

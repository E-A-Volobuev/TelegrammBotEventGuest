using System.Collections.Generic;
using System.Threading.Tasks;

namespace TelegrammBotEventGuest.Core.Interfaces
{
    public interface IBaseRepository<T> where T:class
    {
        Task<List<T>> GetAllAsync();

        Task CreatAsync(T entity);

        Task DeleteAsync(T entity);

        Task UpdateAsync(T entity);

    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mugs.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(uint bookingNumber);
        Task<T> GetItemAsync(uint bookingNumber);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}

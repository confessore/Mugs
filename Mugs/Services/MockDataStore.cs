using Mugs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mugs.Services
{
    public class MockDataStore : IDataStore<Inmate>
    {
        List<Inmate> inmates;

        public MockDataStore()
        {
            inmates = new List<Inmate>();
            var mockInmates = new List<Inmate>
            {
                new Inmate { BookingNumber = 1, Name = "First inmate", Age = 1 },
                new Inmate { BookingNumber = 2, Name = "Second inmate", Age = 2 },
                new Inmate { BookingNumber = 3, Name = "Third inmate", Age = 3 },
                new Inmate { BookingNumber = 4, Name = "Fourth inmate", Age = 4 },
                new Inmate { BookingNumber = 5, Name = "Fifth inmate", Age = 5 },
                new Inmate { BookingNumber = 6, Name = "Sixth inmate", Age = 6 }
            };

            foreach (var inmate in mockInmates)
            {
                inmates.Add(inmate);
            }
        }

        public async Task<bool> AddItemAsync(Inmate inmate)
        {
            inmates.Add(inmate);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Inmate inmate)
        {
            var oldInmate = inmates.Where((Inmate arg) => arg.BookingNumber == inmate.BookingNumber).FirstOrDefault();
            inmates.Remove(oldInmate);
            inmates.Add(inmate);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(uint bookingNumber)
        {
            var oldInmate = inmates.Where((Inmate arg) => arg.BookingNumber == bookingNumber).FirstOrDefault();
            inmates.Remove(oldInmate);

            return await Task.FromResult(true);
        }

        public async Task<Inmate> GetItemAsync(uint bookingNumber)
        {
            return await Task.FromResult(inmates.FirstOrDefault(s => s.BookingNumber == bookingNumber));
        }

        public async Task<IEnumerable<Inmate>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(inmates);
        }
    }
}

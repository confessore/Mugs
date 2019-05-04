using Mugs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mugs.Services.Interfaces
{
    public interface IMySqlQuerier
    {
        Task<List<Inmate>> GetFiveInmates();

        Task<bool> InmateExistsAsync(uint bookingNumber);

        Task InsertInmateAsync(Inmate inmate);

        bool TableExists(string database, string table);

        Task<bool> TableExistsAsync(string database, string table);

        void TryCreateTable(string table, Dictionary<string, string> columns);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns">Key = Column Name, Value = Column Type</param>
        /// <returns></returns>
        Task TryCreateTableAsync(string table, Dictionary<string, string> columns);
    }
}

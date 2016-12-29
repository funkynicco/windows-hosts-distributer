using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Shared.Models;

namespace WHD.DataAccess.Interfaces
{
    public interface IStoreDataAccess
    {
        Task<IEnumerable<Store>> GetStores();

        Task<Store> GetStore(int id);

        Task<bool> SetStoreName(int id, string name);

        Task<int?> AddStore(string name);

        Task<bool> DeleteStore(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;
using WHD.DataAccess.Interfaces;
using WHD.Shared.Models;

namespace WHD.BusinessLogic
{
    public class StoreBusinessLogic : IStoreBusinessLogic
    {
        private readonly IStoreDataAccess _storeDataAccess;

        public StoreBusinessLogic(IStoreDataAccess storeDataAccess)
        {
            _storeDataAccess = storeDataAccess;
        }

        public Task<IEnumerable<Store>> GetStores()
        {
            return _storeDataAccess.GetStores();
        }

        public Task<Store> GetStore(int id)
        {
            return _storeDataAccess.GetStore(id);
        }

        public Task<bool> SetStoreName(int id, string name)
        {
            return _storeDataAccess.SetStoreName(id, name);
        }

        public Task<Store> AddStore(string name)
        {
            return _storeDataAccess.AddStore(name);
        }

        public Task<bool> DeleteStore(int id)
        {
            return _storeDataAccess.DeleteStore(id);
        }
    }
}

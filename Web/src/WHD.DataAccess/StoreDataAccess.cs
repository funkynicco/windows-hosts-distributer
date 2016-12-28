using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WHD.DataAccess.Framework;
using WHD.DataAccess.Interfaces;
using WHD.Shared.Models;

namespace WHD.DataAccess
{
    public class StoreDataAccess : DataAccessBase, IStoreDataAccess
    {
        public StoreDataAccess(IDatabaseSettings databaseSettings) :
            base(databaseSettings.ConnectionString)
        {
        }

        public async Task<IEnumerable<Store>> GetStores()
        {
            using (var query = await Query("[dbo].[GetStores]")
                .CommandType(CommandType.StoredProcedure)
                .ExecuteAsync())
            {
                var stores = new List<Store>();
                while (await query.ReadAsync())
                    stores.Add(Store.FromResult(query));
                return stores;
            }
        }

        public async Task<Store> GetStore(int id)
        {
            using (var query = await Query("[dbo].[GetStores]")
                .CommandType(CommandType.StoredProcedure)
                .AddParameter("id", SqlDbType.Int, id)
                .ExecuteAsync())
            {
                if (await query.ReadAsync())
                    return Store.FromResult(query);

                return null;
            }
        }

        public async Task<bool> SetStoreName(int id, string name)
        {
            var query = Query("[dbo].[SetStoreName]")
                .CommandType(CommandType.StoredProcedure)
                .AddParameter("id", SqlDbType.Int, id)
                .AddParameter("name", SqlDbType.VarChar, name);

            return await query.ExecuteScalarAsync<bool>();
        }

        public async Task<Store> AddStore(string name)
        {
            using (var query = await Query("[dbo].[AddStore]")
               .CommandType(CommandType.StoredProcedure)
               .AddParameter("name", SqlDbType.VarChar, name)
               .ExecuteAsync())
            {
                if (await query.ReadAsync())
                    return Store.FromResult(query);

                return null;
            }
        }

        public async Task<bool> DeleteStore(int id)
        {
            var query = Query("[dbo].[DeleteStore]")
                .CommandType(CommandType.StoredProcedure)
                .AddParameter("id", SqlDbType.Int, id);

            return await query.ExecuteScalarAsync<bool>();
        }
    }
}

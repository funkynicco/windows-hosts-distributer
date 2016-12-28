using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WHD.DataAccess.Framework;
using WHD.DataAccess.Interfaces;
using WHD.Shared.Models;

namespace WHD.DataAccess
{
    public class DomainDataAccess : DataAccessBase, IDomainDataAccess
    {
        public DomainDataAccess(IDatabaseSettings databaseSettings) :
            base(databaseSettings.ConnectionString)
        {
        }

        public async Task<IEnumerable<DomainSimple>> GetDomains()
        {
            using (var query = await Query("[dbo].[GetDomains]")
                .CommandType(CommandType.StoredProcedure)
                .ExecuteAsync())
            {
                var domains = new List<DomainSimple>();
                while (await query.ReadAsync())
                    domains.Add(DomainSimple.FromResult(query));
                return domains;
            }
        }

        public async Task<IEnumerable<DomainSimple>> GetDomainsByStore(int store)
        {
            using (var query = await Query("[dbo].[GetDomains]")
                .CommandType(CommandType.StoredProcedure)
                .AddParameter("store", SqlDbType.Int, store)
                .ExecuteAsync())
            {
                var domains = new List<DomainSimple>();
                while (await query.ReadAsync())
                    domains.Add(DomainSimple.FromResult(query));
                return domains;
            }
        }

        public async Task<DomainDetailed> GetDomainDetails(string domain)
        {
            using (var query = await Query("[dbo].[GetDomainDetails]")
                .CommandType(CommandType.StoredProcedure)
                .AddParameter("domain", SqlDbType.VarChar, domain)
                .ExecuteAsync())
            {
                if (await query.ReadAsync())
                    return DomainDetailed.FromResult(query);

                return null;
            }
        }
    }
}

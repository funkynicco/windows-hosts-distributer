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
    public class HitsDataAccess : DataAccessBase, IHitsDataAccess
    {
        public HitsDataAccess(IDatabaseSettings databaseSettings) :
            base(databaseSettings.ConnectionString)
        {
        }

        public async Task<IEnumerable<Hit>> GetHits()
        {
            using (var query = await Query("[dbo].[GetHits]")
                .CommandType(CommandType.StoredProcedure)
                .ExecuteAsync())
            {
                var hits = new List<Hit>();
                while (await query.ReadAsync())
                    hits.Add(Hit.FromResult(query));
                return hits;
            }
        }
    }
}

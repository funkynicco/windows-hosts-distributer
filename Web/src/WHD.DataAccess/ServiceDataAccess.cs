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
    public class ServiceDataAccess : DataAccessBase, IServiceDataAccess
    {
        public ServiceDataAccess(IDatabaseSettings databaseSettings) :
            base(databaseSettings.ConnectionString)
        {
        }

        public async Task<ServiceStatus> GetServiceStatus()
        {
            using (var query = await Query("[dbo].[GetServiceStatus]")
                .CommandType(CommandType.StoredProcedure)
                .ExecuteAsync())
            {
                if (await query.ReadAsync())
                    return ServiceStatus.FromResult(query);

                return null;
            }
        }
    }
}

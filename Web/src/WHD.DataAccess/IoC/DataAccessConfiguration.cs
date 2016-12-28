using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.DataAccess.Interfaces;

namespace WHD.DataAccess.IoC
{
    public static class DataAccessConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseSettings, DatabaseSettings>();

            services.AddTransient<IStoreDataAccess, StoreDataAccess>();
            services.AddTransient<IDomainDataAccess, DomainDataAccess>();
            services.AddTransient<IServiceDataAccess, ServiceDataAccess>();
        }
    }
}

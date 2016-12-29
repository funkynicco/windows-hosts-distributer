using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;
using WHD.DataAccess.IoC;

namespace WHD.BusinessLogic.IoC
{
    public static class BusinessLogicConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            DataAccessConfiguration.Configure(services);

            services.AddTransient<IStoreBusinessLogic, StoreBusinessLogic>();
            services.AddTransient<IDomainBusinessLogic, DomainBusinessLogic>();
            services.AddTransient<IServiceBusinessLogic, ServiceBusinessLogic>();
            services.AddTransient<IHitsBusinessLogic, HitsBusinessLogic>();
        }
    }
}

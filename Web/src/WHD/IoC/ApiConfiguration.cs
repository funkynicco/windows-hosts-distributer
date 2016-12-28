using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.IoC;

namespace WHD.IoC
{
    public static class ApiConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            BusinessLogicConfiguration.Configure(services);
        }
    }
}

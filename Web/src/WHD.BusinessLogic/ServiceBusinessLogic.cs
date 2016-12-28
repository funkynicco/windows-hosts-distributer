using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;
using WHD.DataAccess.Interfaces;
using WHD.Shared.Models;

namespace WHD.BusinessLogic
{
    public class ServiceBusinessLogic : IServiceBusinessLogic
    {
        private readonly IServiceDataAccess _serviceDataAccess;

        public ServiceBusinessLogic(IServiceDataAccess serviceDataAccess)
        {
            _serviceDataAccess = serviceDataAccess;
        }

        public Task<ServiceStatus> GetServiceStatus()
        {
            return _serviceDataAccess.GetServiceStatus();
        }
    }
}

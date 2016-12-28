using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Shared.Models;

namespace WHD.BusinessLogic.Interfaces
{
    public interface IServiceBusinessLogic
    {
        Task<ServiceStatus> GetServiceStatus();
    }
}

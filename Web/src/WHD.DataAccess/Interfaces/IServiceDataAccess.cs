﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Shared.Models;

namespace WHD.DataAccess.Interfaces
{
    public interface IServiceDataAccess
    {
        Task<ServiceStatus> GetServiceStatus();
    }
}

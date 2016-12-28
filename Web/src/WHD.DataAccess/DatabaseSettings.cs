using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.DataAccess.Interfaces;

namespace WHD.DataAccess
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; private set; }

        public DatabaseSettings(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
    }
}

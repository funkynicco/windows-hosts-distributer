using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WHD.Utility;
using WHD.Utility.Interfaces;

namespace WHD.Shared.Models
{
    public class ServiceStatus
    {
        public ServiceState Status { get; set; }

        public DateTime? Started { get; set; }

        public long DomainsResolved { get; set; }

        public long DomainsFailed { get; set; }

        public long DomainsCached { get; set; }

        public long CacheHits { get; set; }

        public static ServiceStatus FromResult(IQueryResult result)
        {
            return new ServiceStatus()
            {
                Status = result.GetEnum<ServiceState>("Status", SqlDbType.Int),
                Started = result.GetUtcDateTime("Started"),
                DomainsResolved = result.GetInt64("DomainsResolved"),
                DomainsFailed = result.GetInt64("DomainsFailed"),
                DomainsCached = result.GetInt64("DomainsCached"),
                CacheHits = result.GetInt64("CacheHits")
            };
        }
    }
}

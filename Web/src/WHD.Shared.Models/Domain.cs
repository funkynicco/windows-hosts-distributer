using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Utility.Interfaces;

namespace WHD.Shared.Models
{
    public class DomainSimple
    {
        public string Domain { get; set; }

        public int Store { get; set; }

        public string IP { get; set; }

        public DateTime Added { get; set; }

        public static DomainSimple FromResult(IQueryResult result)
        {
            return new DomainSimple()
            {
                Domain = result.GetString("Domain"),
                Store = result.GetInt32("Store"),
                IP = result.GetString("IP"),
                Added = result.GetUtcDateTime("Added").Value
            };
        }
    }

    public class DomainDetailed : DomainSimple
    {
        public string Description { get; set; }

        public long Hits { get; set; }

        public DateTime? LastResolve { get; set; }

        public new static DomainDetailed FromResult(IQueryResult result)
        {
            return new DomainDetailed()
            {
                Domain = result.GetString("Domain"),
                Store = result.GetInt32("Store"),
                IP = result.GetString("IP"),
                Added = result.GetUtcDateTime("Added").Value,

                // extra fields
                Description = result.GetString("Description"),
                Hits = result.GetInt64("Hits"),
                LastResolve = result.GetUtcDateTime("LastResolve")
            };
        }
    }
}

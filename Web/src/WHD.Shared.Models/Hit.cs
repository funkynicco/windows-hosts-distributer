using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Utility.Interfaces;

namespace WHD.Shared.Models
{
    public class Hit
    {
        public string Domain { get; set; }

        public long Hits { get; set; }

        public DateTime LastResolve { get; set; }

        public static Hit FromResult(IQueryResult result)
        {
            return new Hit()
            {
                Domain = result.GetString("Domain"),
                Hits = result.GetInt64("Hits"),
                LastResolve = result.GetUtcDateTime("LastResolve").Value
            };
        }
    }
}

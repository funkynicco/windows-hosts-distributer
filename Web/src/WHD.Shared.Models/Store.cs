using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Utility.Interfaces;

namespace WHD.Shared.Models
{
    public class Store
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Modified { get; set; }

        /// <summary>
        /// Indicates that the store cannot be modified or deleted.
        /// </summary>
        public bool Locked { get; set; }

        public static Store FromResult(IQueryResult result)
        {
            return new Store()
            {
                Id = result.GetInt32("Id"),
                Name = result.GetString("Name"),
                Modified = result.GetUtcDateTime("Modified").Value,
                Locked = result.GetBoolean("Locked")
            };
        }
    }
}

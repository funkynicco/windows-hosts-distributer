using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Shared.Models;

namespace WHD.DataAccess.Interfaces
{
    public interface IDomainDataAccess
    {
        Task<IEnumerable<DomainSimple>> GetDomains();

        Task<IEnumerable<DomainSimple>> GetDomainsByStore(int store);

        Task<DomainDetailed> GetDomainDetails(string domain);
    }
}

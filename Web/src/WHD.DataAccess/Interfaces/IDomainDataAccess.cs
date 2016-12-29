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

        Task<bool> UpdateDomain(string original_domain, int store, string new_domain, string ip, string description);

        Task<bool> AddDomain(int store, string domain, string ip, string description);

        Task<bool> DeleteDomain(string domain);
    }
}

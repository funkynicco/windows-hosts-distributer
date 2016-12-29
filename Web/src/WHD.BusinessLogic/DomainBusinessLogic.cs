using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;
using WHD.DataAccess.Interfaces;
using WHD.Shared.Models;

namespace WHD.BusinessLogic
{
    public class DomainBusinessLogic : IDomainBusinessLogic
    {
        private readonly IDomainDataAccess _domainDataAccess;

        public DomainBusinessLogic(IDomainDataAccess domainDataAccess)
        {
            _domainDataAccess = domainDataAccess;
        }

        public Task<IEnumerable<DomainSimple>> GetDomains()
        {
            return _domainDataAccess.GetDomains();
        }

        public Task<IEnumerable<DomainSimple>> GetDomainsByStore(int store)
        {
            return _domainDataAccess.GetDomainsByStore(store);
        }

        public Task<DomainDetailed> GetDomainDetails(string domain)
        {
            return _domainDataAccess.GetDomainDetails(domain);
        }

        public Task<bool> UpdateDomain(string original_domain, int store, string new_domain, string ip, string description)
        {
            return _domainDataAccess.UpdateDomain(original_domain, store, new_domain, ip, description);
        }

        public Task<bool> AddDomain(int store, string domain, string ip, string description)
        {
            return _domainDataAccess.AddDomain(store, domain, ip, description);
        }

        public Task<bool> DeleteDomain(string domain)
        {
            return _domainDataAccess.DeleteDomain(domain);
        }
    }
}

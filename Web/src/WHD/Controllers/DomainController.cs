using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;

namespace WHD.Controllers
{
    [Route("api/[controller]")]
    public class DomainController : Controller
    {
        private readonly IDomainBusinessLogic _domainBusinessLogic;

        public DomainController(IDomainBusinessLogic domainBusinessLogic)
        {
            _domainBusinessLogic = domainBusinessLogic;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            int page;
            if (!int.TryParse(Request.Query["page"], out page) ||
                page < 1)
                page = 1;

            int itemsPerPage;
            if (!int.TryParse(Request.Query["ipp"], out itemsPerPage) ||
                itemsPerPage < 1 ||
                itemsPerPage > 100)
                itemsPerPage = 15;

            int filter_store;
            if (!int.TryParse(Request.Query["filter-store"], out filter_store))
                filter_store = 0;

            string filter_domain = (string)Request.Query["filter-domain"] ?? "";
            string filter_address = (string)Request.Query["filter-address"] ?? "";

            //Console.WriteLine($"store: {filter_store}, domain: {filter_domain}, address: {filter_address}");

            var domains = await _domainBusinessLogic.GetDomains();

            // apply filters
            domains = domains.Where(a =>
                (filter_store == 0 || a.Store == filter_store) &&
                (filter_domain.Length == 0 || a.Domain.Contains(filter_domain)) &&
                (filter_address.Length == 0 || a.IP.Contains(filter_address)));

            var totalItems = domains.Count();

            domains = domains
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage);

            return Ok(new
            {
                total = totalItems,
                domains = domains
            });
        }

        [HttpGet("{domain}")]
        public async Task<IActionResult> Get(string domain)
        {
            var details = await _domainBusinessLogic.GetDomainDetails(domain);
            if (details == null)
                return NotFound();

            return Ok(details);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;

namespace WHD.Controllers
{
    [Route("api/[controller]")]
    public class HitsController : Controller
    {
        private readonly IHitsBusinessLogic _hitsBusinessLogic;

        public HitsController(IHitsBusinessLogic hitsBusinessLogic)
        {
            _hitsBusinessLogic = hitsBusinessLogic;
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

            string filter_domain = (string)Request.Query["filter-domain"] ?? "";

            var hits = await _hitsBusinessLogic.GetHits();

            // apply filters
            hits = hits.Where(a => filter_domain.Length == 0 || a.Domain.Contains(filter_domain));

            var totalItems = hits.Count();

            hits = hits
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage);

            return Ok(new
            {
                total = totalItems,
                hits = hits
            });
        }
    }
}

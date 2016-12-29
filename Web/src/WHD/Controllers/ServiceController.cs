using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;

namespace WHD.Controllers
{
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        private readonly IServiceBusinessLogic _serviceBusinessLogic;

        public ServiceController(IServiceBusinessLogic serviceBusinessLogic)
        {
            _serviceBusinessLogic = serviceBusinessLogic;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var status = await _serviceBusinessLogic.GetServiceStatus();
            if (!status.LastUpdate.HasValue ||
                (DateTime.UtcNow - status.LastUpdate.Value).TotalMinutes > 5)
                status.Status = 0;

            return Ok(status);
        }
    }
}

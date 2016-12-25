using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WHD.Controllers
{
    [Route("api/[controller]")]
    public class DomainController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { test = 1 });
        }
    }
}

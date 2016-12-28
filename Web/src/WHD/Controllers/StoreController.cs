using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;
using WHD.Models;

namespace WHD.Controllers
{
    [Route("api/[controller]")]
    public class StoreController : Controller
    {
        private readonly IStoreBusinessLogic _storeBusinessLogic;

        public StoreController(IStoreBusinessLogic storeBusinessLogic)
        {
            _storeBusinessLogic = storeBusinessLogic;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _storeBusinessLogic.GetStores());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var store = await _storeBusinessLogic.GetStore(id);
            if (store == null)
                return NotFound();

            return Ok(store);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]AddOrUpdateStoreModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _storeBusinessLogic.SetStoreName(id, model.Name))
                return BadRequest();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AddOrUpdateStoreModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var store = await _storeBusinessLogic.AddStore(model.Name);
            if (store == null)
                return BadRequest();

            return Created("/api/store/" + store.Id, store);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _storeBusinessLogic.DeleteStore(id))
                return NotFound();

            return NoContent();
        }
    }
}

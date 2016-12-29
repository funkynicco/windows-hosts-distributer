using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.BusinessLogic.Interfaces;
using WHD.DataAccess.Interfaces;
using WHD.Shared.Models;

namespace WHD.BusinessLogic
{
    public class HitsBusinessLogic : IHitsBusinessLogic
    {
        private readonly IHitsDataAccess _hitsDataAccess;

        public HitsBusinessLogic(IHitsDataAccess hitsDataAccess)
        {
            _hitsDataAccess = hitsDataAccess;
        }

        public Task<IEnumerable<Hit>> GetHits()
        {
            return _hitsDataAccess.GetHits();
        }
    }
}

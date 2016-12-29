using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Shared.Models;

namespace WHD.BusinessLogic.Interfaces
{
    public interface IHitsBusinessLogic
    {
        Task<IEnumerable<Hit>> GetHits();
    }
}

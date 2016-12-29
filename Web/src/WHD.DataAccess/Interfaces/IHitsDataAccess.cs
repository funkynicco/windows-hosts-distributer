using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WHD.Shared.Models;

namespace WHD.DataAccess.Interfaces
{
    public interface IHitsDataAccess
    {
        Task<IEnumerable<Hit>> GetHits();
    }
}

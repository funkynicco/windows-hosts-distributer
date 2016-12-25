using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer
{
    public static class GlobalRules
    {
        public const string DomainRegex = @"^((\w|-)+)(\.(\w|-)+)*$";
        public const string IPRegex = @"^\d+\.\d+\.\d+\.\d+$";
    }
}

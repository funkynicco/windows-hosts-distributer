using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer.Distributer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketAttribute : Attribute
    {
        public string Header { get; private set; }

        public PacketAttribute(string header)
        {
            Header = header.ToLower();
        }
    }
}

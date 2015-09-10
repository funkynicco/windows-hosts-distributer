using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer.Network
{
    public interface IClient
    {
        int Send(byte[] data, int length);
    }
}

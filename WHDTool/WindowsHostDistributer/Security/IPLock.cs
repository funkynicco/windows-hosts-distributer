using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer.Security
{
    public static class IPLock
    {
        class IPEntry
        {
            public string IP { get; private set; }
            public DateTime ExpireDate { get; private set; }

            public IPEntry(string ip, DateTime expireDate)
            {
                IP = ip;
                ExpireDate = expireDate;
            }
        }

        private static readonly Dictionary<string, IPEntry> _ips = new Dictionary<string, IPEntry>();
        private static readonly object _lock = new object();
        private static readonly TimeSpan LockTime = TimeSpan.FromSeconds(5);

        public static bool IsLocked(string ip)
        {
            var now = DateTime.UtcNow;
            IPEntry entry;
            lock (_lock)
            {
                if (_ips.TryGetValue(ip, out entry))
                {
                    if (entry.ExpireDate > now)
                        return true;

                    _ips.Remove(ip);
                }
            }

            return false;
        }

        public static void AddLock(string ip)
        {
            lock (_lock)
            {
                _ips.Remove(ip); // removes if it exists
                _ips.Add(ip, new IPEntry(ip, DateTime.UtcNow + LockTime));
            }
        }
    }
}

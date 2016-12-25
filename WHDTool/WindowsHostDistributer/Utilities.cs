using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer
{
    public static class Utilities
    {
        public static string FormatNumber(string number, char format = ',')
        {
            var sb = new StringBuilder(number.Length + 8);

            var n = 0;
            for (var i = number.Length - 1; i >= 0; --i)
            {
                if (n > 0 && n % 3 == 0)
                    sb.Append(format);
                ++n;
                sb.Append(number[i]);
            }

            return sb.ToString().Reverse();
        }

        public static string FormatNumber(int number)
        {
            return FormatNumber(number.ToString());
        }
    }
}

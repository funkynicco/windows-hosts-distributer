using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsHostDistributer
{
    public static class ServerConfiguration
    {
        private static readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        private static object _lock = new object();

        public static bool GetValue(string key, out string result)
        {
            string val;
            if (!_values.TryGetValue(key.ToLower(), out val))
            {
                result = null;
                return false;
            }

            result = val;
            return true;
        }

        public static bool GetValue(string key, out int result)
        {
            result = 0;

            string val;
            if (!GetValue(key, out val))
                return false;

            int ret;
            if (!int.TryParse(val, out ret))
                return false;

            result = ret;
            return true;
        }

        public static bool Load(string filename)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(filename);
            }
            catch
            {
                return false;
            }

            lock (_lock)
            {
                var node = doc.SelectSingleNode("Server");
                if (node == null)
                    return false;

                _values.Clear();

                foreach (XmlNode child in node.ChildNodes)
                {
                    var name = child.Name.ToLower();
                    if (name == "keyfile")
                    {
                        // load keyfile
                        _values.Add("keydata", File.ReadAllText(child.InnerText).Replace("\r", ""));
                    }
                    else
                        _values.Add(name, child.InnerText);
                }
            }

            return true;
        }
    }
}

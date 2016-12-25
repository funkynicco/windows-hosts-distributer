using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsHostDistributer
{
    public class HostEntry
    {
        public string Name { get; private set; }
        public string IP { get; private set; }
        public string Description { get; private set; }
        public bool Hidden { get; private set; }

        public HostEntry(string name, string ip, string description, bool hidden)
        {
            Name = name;
            IP = ip;
            Description = description;
            Hidden = hidden;
        }
    }

    public static class HostsDatabase
    {
        private static readonly Dictionary<string, HostEntry> _hosts = new Dictionary<string, HostEntry>();
        private static readonly Dictionary<string, HostEntry> _localHosts = new Dictionary<string, HostEntry>();
        private static readonly object _lock = new object();

        public static IEnumerable<HostEntry> Hosts { get { return _hosts.Values; } }
        public static IEnumerable<HostEntry> LocalHosts { get { return _localHosts.Values; } }

        #region Lock, TryLock and Unlock
        public static void Lock()
        {
            Monitor.Enter(_lock);
        }

        public static bool TryLock()
        {
            return Monitor.TryEnter(_lock);
        }

        public static void Unlock()
        {
            Monitor.Exit(_lock);
        }
        #endregion

        #region Load, ReadNode and Save
        public static bool Load()
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load("hosts.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            var node = doc.SelectSingleNode("Database/Hosts");
            if (node == null)
                return false;

            lock (_lock)
            {
                _hosts.Clear();
                _localHosts.Clear();

                ReadNode(node, _hosts);

                if ((node = doc.SelectSingleNode("Database/LocalHosts")) != null)
                    ReadNode(node, _localHosts);
            }

            return true;
        }

        private static void ReadNode(XmlNode node, IDictionary<string, HostEntry> dictionary)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                var nameAttribute = child.Attributes["Name"];
                var ipAttribute = child.Attributes["IP"];
                var descriptionAttribute = child.Attributes["Description"];
                var hiddenAttribute = child.Attributes["Hidden"];

                if (nameAttribute != null &&
                    ipAttribute != null &&
                    !string.IsNullOrWhiteSpace(nameAttribute.Value) &&
                    !string.IsNullOrWhiteSpace(ipAttribute.Value))
                {
                    var name = nameAttribute.Value.ToLower();

                    if (!dictionary.ContainsKey(name))
                    {
                        var description = descriptionAttribute != null ? descriptionAttribute.Value : string.Empty;
                        var isHidden = hiddenAttribute != null && string.Compare(hiddenAttribute.Value, "true", true) == 0;

                        dictionary.Add(name, new HostEntry(name, ipAttribute.Value, description, isHidden));
                    }
                }
            }
        }

        public static void Save()
        {
            var sb = new StringBuilder(1024);

            lock (_lock)
            {
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine();
                sb.AppendLine("<Database>");
                sb.AppendLine("\t<!-- The following hosts are remote hosts located on the server. -->");
                sb.AppendLine("\t<Hosts>");
                foreach (var kv in _hosts)
                {
                    sb.AppendLine(string.Format(
                        "\t\t<Host Name=\"{0}\" IP=\"{1}\" Description=\"{2}\"{3} />",
                        kv.Value.Name.Escape(EscapeLanguage.Xml),
                        kv.Value.IP.Escape(EscapeLanguage.Xml),
                        kv.Value.Description.Escape(EscapeLanguage.Xml),
                        kv.Value.Hidden ? " Hidden=\"True\"" : ""));
                }
                sb.AppendLine("\t</Hosts>");
                sb.AppendLine();
                sb.AppendLine("\t<!-- The following hosts are local to a client computer only and thus not overwritten by the servers hosts. -->");
                sb.AppendLine("\t<LocalHosts>");
                foreach (var kv in _localHosts)
                {
                    sb.AppendLine(string.Format(
                        "\t\t<Host Name=\"{0}\" IP=\"{1}\" Description=\"{2}\" />",
                        kv.Value.Name.Escape(EscapeLanguage.Xml),
                        kv.Value.IP.Escape(EscapeLanguage.Xml),
                        kv.Value.Description.Escape(EscapeLanguage.Xml)));
                }
                sb.AppendLine("\t</LocalHosts>");
                sb.Append("</Database>");
            }

            File.WriteAllText("hosts.xml", sb.ToString());
        }
        #endregion

        public static void Clear(bool local)
        {
            var dictionary = local ? _localHosts : _hosts;
            lock (_lock)
            {
                dictionary.Clear();
            }
        }

        public static void Add(bool local, string name, string ip, string description, bool hidden)
        {
            var dictionary = local ? _localHosts : _hosts;
            lock (_lock)
            {
                dictionary[name.ToLower()] = new HostEntry(name, ip, description, hidden);
            }
        }

        public static bool Remove(bool local, string name)
        {
            var dictionary = local ? _localHosts : _hosts;
            lock (_lock)
            {
                return dictionary.Remove(name.ToLower());
            }
        }

        public static bool Contains(bool local, string name)
        {
            var dictionary = local ? _localHosts : _hosts;
            lock (_lock)
            {
                return dictionary.ContainsKey(name.ToLower());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsHostDistributer.Security;

namespace WindowsHostDistributer.Distributer
{
    public partial class DistributerServer
    {
        [Packet("login")]
        void OnLogin(UserClient client, string data)
        {
            string server_keydata = string.Empty;
            if (!ServerConfiguration.GetValue("keydata", out server_keydata))
            {
                client.Disconnect("Server keydata is missing...");
                return;
            }

            if (data.Length != server_keydata.Length ||
                string.Compare(data, server_keydata) != 0)
            {
                IPLock.AddLock(client.NetworkClient.IP);
                client.Disconnect("Client keydata is invalid");
                return;
            }

            client.IsAuthorized = true;
            client.NetworkClient.Send("logedin\n");
            Logger.Log(LogType.Debug, "{0} successfully authorized.", client.NetworkClient.IP);

            // send hosts list and shit
            var sb = new StringBuilder();
            sb.Append("hosts ");
            int i = 0;
            HostsDatabase.Lock();
            foreach (var host in HostsDatabase.Hosts)
            {
                if (i++ > 0)
                    sb.Append('|');

                sb.AppendFormat(
                    "{0}/{1}/{2}/{3}",
                    host.Name.Escape(EscapeLanguage.Xml),
                    host.IP.Escape(EscapeLanguage.Xml),
                    host.Description.Escape(EscapeLanguage.Xml),
                    host.Hidden ? "1" : "0");
            }
            HostsDatabase.Unlock();
            sb.Append('\n');

            client.NetworkClient.Send(sb.ToString()); // host list
        }

        [Packet("ping")]
        void OnPing(UserClient client, string data)
        {
        }

        [Packet("add-domain")]
        void OnAddDomain(UserClient client, string data)
        {
            var param = data.Split('/');
            if (param.Length == 4)
            {
                HostsDatabase.Add(false,
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml),
                    param[2].Unescape(EscapeLanguage.Xml),
                    param[3] == "1");
                HostsDatabase.Save();

                Logger.Log(
                    LogType.Debug,
                    "Added domain {0} = {1}",
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml));

                BroadcastPacket("add-domain {0}\n", data);
            }
        }

        [Packet("edit-domain")]
        void OnEditDomain(UserClient client, string data)
        {
            var param = data.Split('/');
            if (param.Length == 4)
            {
                var added = !HostsDatabase.Remove(false, param[0].Unescape(EscapeLanguage.Xml));
                HostsDatabase.Add(false,
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml),
                    param[2].Unescape(EscapeLanguage.Xml),
                    param[3] == "1");
                HostsDatabase.Save();

                Logger.Log(
                    LogType.Debug,
                    "{0} domain {1} = {2}",
                    added ? "Added" : "Edited",
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml));

                BroadcastPacket("edit-domain {0}\n", data);
            }
        }

        [Packet("remove-domain")]
        void OnRemoveDomain(UserClient client, string data)
        {
            if (HostsDatabase.Remove(false, data.Unescape(EscapeLanguage.Xml)))
            {
                HostsDatabase.Save();
                Logger.Log(LogType.Debug, "Removed domain {0}", data.Unescape(EscapeLanguage.Xml));
                BroadcastPacket("remove-domain {0}\n", data);
            }
            else
                Logger.Log(LogType.Warning, "OnRemoveDomain from {0}: Domain '{1}' not found. [Action ignored]", client.NetworkClient.IP, data.Unescape(EscapeLanguage.Xml));
        }
    }
}

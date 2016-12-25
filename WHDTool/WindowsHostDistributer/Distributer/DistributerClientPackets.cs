using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer.Distributer
{
    public partial class DistributerClient
    {
        [Packet("logedin")]
        void OnLogedin(string data)
        {
            _isAuthorized = true;

            EventCallbackSystem.InvokeCallback("ClientAuthorized");
        }

        [Packet("hosts")]
        void OnHosts(string data)
        {
            HostsDatabase.Clear(false);

            foreach (var entry in data.Split('|'))
            {
                var param = entry.Split('/');
                if (param.Length == 4)
                {
                    HostsDatabase.Add(false,
                        param[0].Unescape(EscapeLanguage.Xml),
                        param[1].Unescape(EscapeLanguage.Xml),
                        param[2].Unescape(EscapeLanguage.Xml),
                        param[3] == "1");
                }
            }

            HostsDatabase.Save();
            EventCallbackSystem.InvokeCallback("HostsListUpdated", true);

            HostsDatabase.Lock();
            int numberOfHosts = HostsDatabase.Hosts.Count();
            HostsDatabase.Unlock();
            Logger.Log(LogType.Debug, "Received {0} hosts from server.", numberOfHosts.FormatNumber());
        }

        [Packet("add-domain")]
        void OnAddDomain(string data)
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
                EventCallbackSystem.InvokeCallback("HostsListUpdated", true);

                Logger.Log(LogType.Debug,
                    "Added domain {0} = {1}",
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml));
            }
        }

        [Packet("edit-domain")]
        void OnEditDomain(string data)
        {
            var param = data.Split('/');
            if (param.Length == 4)
            {
                HostsDatabase.Remove(false, param[0].Unescape(EscapeLanguage.Xml));
                HostsDatabase.Add(false,
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml),
                    param[2].Unescape(EscapeLanguage.Xml),
                    param[3] == "1");
                HostsDatabase.Save();
                EventCallbackSystem.InvokeCallback("HostsListUpdated", true);

                Logger.Log(LogType.Debug,
                    "Edited domain {0} = {1}",
                    param[0].Unescape(EscapeLanguage.Xml),
                    param[1].Unescape(EscapeLanguage.Xml));
            }
        }

        [Packet("remove-domain")]
        void OnRemoveDomain(string data)
        {
            if (HostsDatabase.Remove(false, data.Unescape(EscapeLanguage.Xml)))
            {
                HostsDatabase.Save();
                EventCallbackSystem.InvokeCallback("HostsListUpdated", true);

                Logger.Log(LogType.Debug,
                    "Removed domain {0}",
                    data.Unescape(EscapeLanguage.Xml));
            }
        }

        // send
        public void SendAddDomain(string domainName, string ipAddress, string description, bool hidden)
        {
            Send(string.Format(
                "add-domain {0}/{1}/{2}/{3}\n",
                domainName.Escape(EscapeLanguage.Xml),
                ipAddress.Escape(EscapeLanguage.Xml),
                description.Escape(EscapeLanguage.Xml),
                hidden ? "1" : "0"));
        }

        public void SendEditDomain(string domainName, string ipAddress, string description, bool hidden)
        {
            Send(string.Format(
                "edit-domain {0}/{1}/{2}/{3}\n",
                domainName.Escape(EscapeLanguage.Xml),
                ipAddress.Escape(EscapeLanguage.Xml),
                description.Escape(EscapeLanguage.Xml),
                hidden ? "1" : "0"));
        }

        public void SendRemoveDomain(string domainName)
        {
            Send(string.Format("remove-domain {0}\n", domainName.Escape(EscapeLanguage.Xml)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsHostDistributer.Network;
using WindowsHostDistributer.Security;

namespace WindowsHostDistributer.Distributer
{
    public partial class DistributerServer : NetworkServerBase
    {
        class UserClient : IDisposable
        {
            public Client NetworkClient { get; private set; }
            public DateTime LastActivity { get; set; }
            public bool IsAuthorized { get; set; }
            public MemoryStream Buffer { get; private set; }

            public UserClient(Client networkClient)
            {
                NetworkClient = networkClient;
                LastActivity = DateTime.UtcNow;
                Buffer = new MemoryStream(1024);
            }

            public void Dispose()
            {
            }

            public void Disconnect()
            {
                NetworkClient.Disconnect();
            }

            public void Disconnect(string reason)
            {
                NetworkClient.Disconnect(reason);
            }
        }

        private readonly Dictionary<string, MethodInfo> _packetMethods = new Dictionary<string, MethodInfo>();
        private int _nextCheckActivity = 0;

        public DistributerServer()
        {
            foreach (var mi in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attribute = mi.GetCustomAttribute<PacketAttribute>();
                if (attribute != null)
                    _packetMethods.Add(attribute.Header, mi);
            }
        }

        protected override void OnClientConnected(Client client)
        {
            if (IPLock.IsLocked(client.IP))
            {
                client.Disconnect();
                return; // client.Tag is null so there is no OnClientDisconnected code running for it
            }

            client.Tag = new UserClient(client);

            Logger.Log(LogType.Debug, "[{0}-{1}] Client connected", client.IP, client.SocketHandle.ToInt32());
        }

        protected override void OnClientDisconnected(Client client)
        {
            if (client.Tag != null) // if it's null then most likely the IP is locked
            {
                if (!string.IsNullOrEmpty(client.DisconnectReason))
                    Logger.Log(LogType.Warning, "[{0}-{1}] Client kicked: {2}", client.IP, client.SocketHandle.ToInt32(), client.DisconnectReason);
                else
                    Logger.Log(LogType.Debug, "[{0}-{1}] Client disconnected", client.IP, client.SocketHandle.ToInt32());
                (client.Tag as UserClient).Dispose();
            }
        }

        protected override void OnClientData(Client client, byte[] buffer, int length)
        {
            var userClient = client.Tag as UserClient;

            userClient.LastActivity = DateTime.UtcNow;

            userClient.Buffer.Position = userClient.Buffer.Length;
            userClient.Buffer.Write(buffer, 0, length);

            int pos;
            while ((pos = userClient.Buffer.Find('\n')) != -1)
            {
                var data = Encoding.GetEncoding(1252).GetString(userClient.Buffer.Extract(0, pos + 1));
                data = data.Substring(0, data.Length - 1);
                if (data.EndsWith("\r"))
                    data = data.Substring(0, data.Length - 1);

                var header = string.Empty;
                if ((pos = data.IndexOf(' ')) != -1)
                {
                    header = data.Substring(0, pos);
                    data = data.Substring(pos + 1);
                }
                else
                {
                    header = data;
                    data = string.Empty;
                }

                header.ToLower();

                MethodInfo methodInfo;
                if (!_packetMethods.TryGetValue(header, out methodInfo))
                {
                    client.Disconnect("Unknown packet header: " + header);
                    return;
                }

                try
                {
                    methodInfo.Invoke(this, new object[] { userClient, data });
                }
                catch (DataValidationException ex)
                {
                    client.Disconnect(ex.Message);
                }

                if (client.IsDisconnect)
                    return;
            }
        }

        public override void Process()
        {
            base.Process();

            int tick = Environment.TickCount;
            if (tick >= _nextCheckActivity)
            {
                _nextCheckActivity = tick + 1000;

                var now = DateTime.UtcNow;

                foreach (var client in Clients)
                {
                    var userClient = client.Tag as UserClient;

                    if ((now - userClient.LastActivity).TotalSeconds >= (userClient.IsAuthorized ? 60 : 15))
                    {
                        Logger.Log(
                            LogType.Warning,
                            "Disconnecting {0}-{1} due to 15 seconds inactivity.",
                            client.IP,
                            client.SocketHandle.ToInt32());

                        client.Disconnect();
                    }
                }
            }
        }

        public void BroadcastPacket(string text)
        {
            foreach (var client in Clients)
            {
                var userClient = client.Tag as UserClient;
                if (userClient.IsAuthorized)
                    userClient.NetworkClient.Send(text);
            }
        }

        public void BroadcastPacket(string format, params object[] args)
        {
            BroadcastPacket(string.Format(format, args));
        }
    }
}

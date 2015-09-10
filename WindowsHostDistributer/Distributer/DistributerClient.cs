using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsHostDistributer.Network;

namespace WindowsHostDistributer.Distributer
{
    public partial class DistributerClient : NetworkClientBase
    {
        private readonly Dictionary<string, MethodInfo> _packetMethods = new Dictionary<string, MethodInfo>();
        private readonly MemoryStream _buffer = new MemoryStream(1024);

        private bool _isAuthorized = false;
        public bool IsAuthorized { get { return _isAuthorized; } }
        private int _nextSendPing = 0;

        public DistributerClient()
        {
            foreach (var mi in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attribute = mi.GetCustomAttribute<PacketAttribute>();
                if (attribute != null)
                    _packetMethods.Add(attribute.Header, mi);
            }
        }

        private int Send(string text)
        {
            var data = Encoding.GetEncoding(1252).GetBytes(text);
            return Send(data, data.Length);
        }

        protected override void OnConnected()
        {
            Logger.Log(LogType.Debug, "Connected to server.");
            Send("login " + Configuration.Key + "\n");
            _nextSendPing = Environment.TickCount + 5000;
        }

        protected override void OnDisconnected()
        {
            if (!string.IsNullOrWhiteSpace(DisconnectMessage))
                Logger.Log(LogType.Debug, "Connection closed. Reason: {0}", DisconnectMessage);
            else
                Logger.Log(LogType.Debug, "Connection closed.");
        }

        protected override void OnData(byte[] buffer, int length)
        {
            _buffer.Position = _buffer.Length;
            _buffer.Write(buffer, 0, length);

            int pos;
            while ((pos = _buffer.Find('\n')) != -1)
            {
                var data = Encoding.GetEncoding(1252).GetString(_buffer.Extract(0, pos + 1));
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
                    Close("Unknown packet header: " + header);
                    return;
                }

                try
                {
                    methodInfo.Invoke(this, new object[] { data });
                }
                catch (DataValidationException ex)
                {
                    Close(ex.Message);
                }

                if (!IsConnected)
                    return;
            }
        }

        public override void Process()
        {
            base.Process();

            if (IsConnected)
            {
                int tick = Environment.TickCount;
                if (tick >= _nextSendPing)
                {
                    _nextSendPing = tick + 5000;

                    Send("ping\n");
                }
            }
        }
    }
}

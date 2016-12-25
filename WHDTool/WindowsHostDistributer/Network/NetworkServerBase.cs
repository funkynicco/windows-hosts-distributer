﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer.Network
{
    public class Client : IDisposable, IClient
    {
        private readonly NetworkServerBase _server;
        private readonly Socket _socket;
        private bool _isDisconnect = false;
        private string _disconnectReason = null;

        public string IP { get; private set; }
        public IntPtr SocketHandle { get; private set; }

        public Socket Socket { get { return _socket; } }

        public bool IsDisconnect { get { return _isDisconnect; } }
        public string DisconnectReason { get { return _disconnectReason; } }

        public object Tag { get; set; }

        public Client(NetworkServerBase server, Socket socket)
        {
            _server = server;
            _socket = socket;

            IP = _socket.RemoteEndPoint.ToString().Split(':')[0];
            SocketHandle = _socket.Handle;
        }

        public void Dispose()
        {
            try { _socket.Dispose(); }
            catch { }
        }

        public void Disconnect(string reason)
        {
            _isDisconnect = true;
            _disconnectReason = reason;
        }

        public void Disconnect()
        {
            Disconnect(null);
        }

        public int Send(byte[] data, int offset, int length)
        {
            return Socket.Send(data, offset, length, SocketFlags.None);
        }

        public int Send(byte[] data, int length)
        {
            return Send(data, 0, length);
        }

        public int Send(string text)
        {
            var data = Encoding.GetEncoding(1252).GetBytes(text);
            return Send(data, 0, data.Length);
        }
    }

    public class NetworkServerBase : IDisposable
    {
        private List<Client> _clients = new List<Client>(10);
        private Socket _socket = null;
        private byte[] _buffer = new byte[65536];

        public IEnumerable<Client> Clients { get { return _clients; } }

        public void Dispose()
        {
            Close();
            OnDispose();
        }

        public bool Start(EndPoint endPoint)
        {
            if (_socket != null)
                return false;

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(endPoint);
                _socket.Listen(100);
                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }

        public bool Start(int port)
        {
            return Start(new IPEndPoint(IPAddress.Any, port));
        }

        public void Close()
        {
            foreach (var client in _clients)
            {
                OnClientDisconnected(client);
                client.Dispose();
            }
            _clients.Clear();

            if (_socket != null)
            {
                try { _socket.Dispose(); }
                catch { }
                _socket = null;
            }
        }

        public virtual void Process()
        {
            if (_socket != null)
            {
                Socket c;
                bool poll;
                try { poll = _socket.Poll(1, SelectMode.SelectRead); }
                catch { poll = false; }

                if (poll)
                {
                    try { c = _socket.Accept(); }
                    catch { c = null; }

                    if (c != null)
                    {
                        var client = new Client(this, c);
                        _clients.Add(client);
                        OnClientConnected(client);
                    }
                }

                for (int i = 0; i < _clients.Count;)
                {
                    var client = _clients[i];

                    int len = 0;
                    if (!client.IsDisconnect) // len is 0 by default so it will be disconnected if IsDisconnect is true
                    {
                        try { poll = client.Socket.Poll(1, SelectMode.SelectRead); }
                        catch { poll = false; client.Disconnect(); }

                        if (!poll)
                        {
                            ++i;
                            continue;
                        }

                        try
                        {
                            len = client.Socket.Receive(_buffer);
                        }
                        catch
                        {
                            len = 0;
                        }
                    }

                    if (len > 0)
                    {
                        // process data
                        OnClientData(client, _buffer, len);
                    }
                    else
                    {
                        OnClientDisconnected(client);

                        client.Dispose();
                        _clients.RemoveAt(i);
                        continue;
                    }

                    ++i;
                }
            }
        }

        protected virtual void OnDispose()
        {
        }

        protected virtual void OnClientConnected(Client client)
        {
        }

        protected virtual void OnClientDisconnected(Client client)
        {
        }

        protected virtual void OnClientData(Client client, byte[] buffer, int length)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Ether.Network.Tests.Helpers;

namespace Ether.Network.Tests.Contexts.Echo
{
    public class MyEchoClient : NetClient
    {
        private readonly Random _random;
        private readonly List<string> _sendedData;

        public ICollection<string> SendedData
        {
            get
            {
                this._sendedData.Sort(StringComparer.CurrentCulture);
                return this._sendedData;
            }
        }

        public bool ConnectedToServer { get; private set; }

        public MyEchoClient(string host, int port, int bufferSize)
        {
            this.Configuration.Host = host;
            this.Configuration.Port = port;
            this.Configuration.BufferSize = bufferSize;
            this._sendedData = new List<string>();
            this._random = new Random();
        }

        protected override void OnConnected()
        {
            this.ConnectedToServer = true;
        }

        protected override void OnDisconnected()
        {
            this.ConnectedToServer = false;
        }

        protected override void OnSocketError(SocketError socketError)
        {
            // Nothing to do.
        }

        public override void HandleMessage(INetPacketStream packet)
        {
            // Nothing to do.
        }

        public void SendRandomMessage()
        {
            string message = $"{Helper.GenerateRandomString(this._random.Next(50))}";

            this._sendedData.Add(message);

            using (var packet = new NetPacket())
            {
                packet.Write(message);
                this.Send(packet);
            }
        }
    }
}

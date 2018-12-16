using System;
using System.Collections.Generic;
using Ether.Network.Common;
using Ether.Network.Packets;
using Ether.Network.Server;

namespace Ether.Network.Tests.Contexts.Echo
{
    public class MyEchoServer : NetServer<EchoClient>
    {
        public MyEchoServer()
        {
            this.Configuration.BufferSize = 512;
            this.Configuration.MaximumNumberOfConnections = 10;
            this.Configuration.Host = "127.0.0.1";
            this.Configuration.Port = 4444;
            this.Configuration.Backlog = 10;
            this.Configuration.Blocking = false;
        }

        protected override void Initialize()
        {
            // Nothing to do.
        }

        protected override void OnClientConnected(EchoClient connection)
        {
            // Nothing to do.
        }

        protected override void OnClientDisconnected(EchoClient connection)
        {
            // Nothing to do.
        }

        protected override void OnError(Exception exception)
        {
            // Nothing to do.
        }
    }

    public class EchoClient : NetUser
    {
        private readonly List<string> _receivedData;

        public ICollection<string> ReceivedData
        {
            get
            {
                this._receivedData.Sort(StringComparer.CurrentCulture);
                return this._receivedData;
            }
        }

        public EchoClient()
        {
            this._receivedData = new List<string>();
        }

        public override void HandleMessage(INetPacketStream packet)
        {
            var receivedString = packet.Read<string>();

            this._receivedData.Add(receivedString);
        }
    }
}

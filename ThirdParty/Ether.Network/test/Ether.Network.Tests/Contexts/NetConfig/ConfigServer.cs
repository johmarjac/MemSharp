using System;
using Ether.Network.Common;
using Ether.Network.Server;

namespace Ether.Network.Tests.Contexts.NetConfig
{
    public class ConfigServer : NetServer<Client>
    {
        protected override void Initialize()
        {
        }

        protected override void OnClientConnected(Client connection)
        {
        }

        protected override void OnClientDisconnected(Client connection)
        {
        }

        protected override void OnError(Exception exception)
        {
        }

        public void SetupConfiguration()
        {
            this.Configuration.BufferSize = 512;
            this.Configuration.MaximumNumberOfConnections = 10;
            this.Configuration.Host = "127.0.0.1";
            this.Configuration.Port = 4445;
            this.Configuration.Backlog = 10;
            this.Configuration.Blocking = false;
        }
    }

    public class Client : NetUser
    {
       
    }
}

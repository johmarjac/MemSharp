using Ether.Network.Server;
using System;

namespace SampleServer
{
    internal sealed class SampleServer : NetServer<Client>
    {
        /// <summary>
        /// Creates a new <see cref="SampleServer"/> with a default configuration.
        /// </summary>
        public SampleServer()
        {
            this.Configuration.Backlog = 100;
            this.Configuration.Port = 4444;
            this.Configuration.MaximumNumberOfConnections = 100;
            this.Configuration.Host = "127.0.0.1";
            this.Configuration.BufferSize = 8;
            this.Configuration.Blocking = true;
        }

        /// <summary>
        /// Initialize the server resources if needed...
        /// </summary>
        protected override void Initialize()
        {
            Console.WriteLine("Server is ready.");
        }

        /// <summary>
        /// On client connected.
        /// </summary>
        /// <param name="connection"></param>
        protected override void OnClientConnected(Client connection)
        {
            Console.WriteLine("New client connected!");

            connection.SendFirstPacket();
        }

        /// <summary>
        /// On client disconnected.
        /// </summary>
        /// <param name="connection"></param>
        protected override void OnClientDisconnected(Client connection)
        {
            Console.WriteLine("Client disconnected!");
        }

        /// <summary>
        /// On server error.
        /// </summary>
        /// <param name="exception"></param>
        protected override void OnError(Exception exception)
        {
            // TBA
        }
    }
}
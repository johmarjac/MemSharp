using Ether.Network.Client;
using Ether.Network.Packets;
using System;
using System.Net.Sockets;

namespace SampleClient
{
    internal sealed class MyClient : NetClient
    {
        /// <summary>
        /// Creates a new <see cref="MyClient"/>.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        public MyClient(string host, int port, int bufferSize, int timeOut)
        {
            this.Configuration.Host = host;
            this.Configuration.Port = port;
            this.Configuration.BufferSize = bufferSize;
            this.Configuration.TimeOut = timeOut;
        }

        /// <summary>
        /// Handles incoming messages.
        /// </summary>
        /// <param name="packet"></param>
        public override void HandleMessage(INetPacketStream packet)
        {
            var response = packet.Read<string>();
            Console.WriteLine($"-> Server response: '{response}'");
        }

        /// <summary>
        /// Triggered when connected to the server.
        /// </summary>
        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", this.Socket.RemoteEndPoint);
        }

        /// <summary>
        /// Triggered when disconnected from the server.
        /// </summary>
        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected");
        }

        /// <summary>
        /// Triggered when an error occurs.
        /// </summary>
        /// <param name="socketError"></param>
        protected override void OnSocketError(SocketError socketError)
        {
            Console.WriteLine("Socket Error: {0}", socketError.ToString());
        }
    }
}
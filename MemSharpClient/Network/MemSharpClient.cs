using System;
using System.Net.Sockets;
using Ether.Network.Client;

namespace MemSharpClient.Network
{
    public class MemSharpClient : NetClient
    {
        public MemSharpClient()
        {
            Configuration.Host = "127.0.0.1";
            Configuration.Port = 31010;
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to MemSharpServer!");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected from MemSharpServer!");
        }

        protected override void OnSocketError(SocketError socketError)
        {
            Console.WriteLine($"Socket Error: {Enum.GetName(typeof(SocketError), socketError)}");
        }
    }
}
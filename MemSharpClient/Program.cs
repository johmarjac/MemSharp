using Ether.Network.Packets;
using System;

namespace MemSharpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new Network.MemSharpClient())
            {
                client.Connect();

                if (!client.IsConnected)
                {
                    Console.WriteLine("Unable to connect to MemSharpServer!");
                    Console.ReadLine();
                    return;
                }

                Console.ReadLine();

                using (var packet = new NetPacket())
                {
                    packet.Write((ushort)0);

                    client.Send(packet);
                }
                
                Console.ReadLine();

                client.Disconnect();
            }

            Console.WriteLine("MemSharpClient will now exit!");
            Console.ReadLine();
        }
    }
}
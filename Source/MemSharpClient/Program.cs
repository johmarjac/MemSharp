using Ether.Network.Packets;
using System;

namespace MemSharpClient
{
    class Program
    {
        enum OpCode : ushort
        {
            Shutdown,
            SetWorkingDirectory,
            StartScriptDomain,
            StopScriptDomain
        }

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

                Console.WriteLine("Setting Working Directory...");
                Console.ReadLine();

                using (var packet = new NetPacket())
                {
                    packet.Write((ushort)OpCode.SetWorkingDirectory);
                    packet.Write(@"E:\Desktop\IgniteBot");

                    client.Send(packet);
                }

                Console.ReadLine();

                Console.WriteLine("Starting ScriptDomain...");
                Console.ReadLine();

                using (var packet = new NetPacket())
                {
                    packet.Write((ushort)OpCode.StartScriptDomain);

                    client.Send(packet);
                }

                Console.ReadLine();

                Console.WriteLine("Stopping ScriptDomain...");
                Console.ReadLine();

                using (var packet = new NetPacket())
                {
                    packet.Write((ushort)OpCode.StopScriptDomain);

                    client.Send(packet);
                }

                Console.ReadLine();

                Console.WriteLine("Shuting down MemSharpServer...");
                Console.ReadLine();

                using (var packet = new NetPacket())
                {
                    packet.Write((ushort)OpCode.Shutdown);

                    client.Send(packet);
                }

                client.Disconnect();
            }

            Console.WriteLine("MemSharpClient will now exit!");
            Console.ReadLine();
        }
    }
}
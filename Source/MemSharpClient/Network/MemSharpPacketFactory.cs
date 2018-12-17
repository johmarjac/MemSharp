using Ether.Network.Client;
using Ether.Network.Packets;
using MemSharpCommon;
using System.Threading;

namespace MemSharpClient.Network
{
    public static class MemSharpPacketFactory
    {
        public static void SendWorkingDirectory(INetClient client, string workingDirectory)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((ushort)OpCode.SetWorkingDirectory);
                packet.Write(workingDirectory);

                client.Send(packet);
                Thread.Sleep(1000);
            }
        }

        public static void SendStartScriptDomain(INetClient client)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((ushort)OpCode.StartScriptDomain);

                client.Send(packet);
                Thread.Sleep(1000);
            }
        }

        public static void SendStopScriptDomain(INetClient client)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((ushort)OpCode.StopScriptDomain);

                client.Send(packet);
                Thread.Sleep(1000);
            }
        }

        public static void SendShutdownServer(INetClient client)
        {
            using (var packet = new NetPacket())
            {
                packet.Write((ushort)OpCode.Shutdown);

                client.Send(packet);
            }
        }
    }
}
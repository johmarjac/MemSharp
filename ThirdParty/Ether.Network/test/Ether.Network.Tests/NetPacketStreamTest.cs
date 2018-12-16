using Ether.Network.Packets;
using Ether.Network.Tests.Helpers;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Ether.Network.Tests
{
    public class NetPacketStreamTest
    {
        private static readonly byte ByteValue = 145;
        private static readonly short ShortValue = 30546;
        private static readonly int Int32Value = 452674652;
        private static readonly long Int64Value = 3465479740298342;
        private static readonly string StringValue = Helper.GenerateRandomString(543);

        private static readonly byte[] ByteTestArray = new byte[] { ByteValue };
        private static readonly byte[] ShortTestArray = BitConverter.GetBytes(ShortValue);
        private static readonly byte[] Int32TestArray = BitConverter.GetBytes(Int32Value);
        private static readonly byte[] Int64TestArray = BitConverter.GetBytes(Int64Value);
        private static readonly byte[] StringTestArray = Encoding.ASCII.GetBytes(StringValue);

        private static readonly byte[] ByteArrayValue = BitConverter.GetBytes(0xDEADBEEF);

        [Fact]
        public void ReadByte()
        {
            this.TestRead<byte>(ByteTestArray, ByteValue);
        }

        [Fact]
        public void ReadInt16()
        {
            this.TestRead<short>(ShortTestArray, ShortValue);
        }

        [Fact]
        public void ReadInt32()
        {
            this.TestRead<int>(Int32TestArray, Int32Value);
        }

        [Fact]
        public void ReadInt64()
        {
            this.TestRead<long>(Int64TestArray, Int64Value);
        }

        [Fact]
        public void ReadString()
        {
            byte[] value = null;

            using (INetPacketStream packetStream = new NetPacketStream(StringTestArray))
                value = packetStream.ReadArray<byte>(StringTestArray.Length);

            string convertedValue = Encoding.ASCII.GetString(value);

            Assert.Equal(StringValue, convertedValue);
        }

        [Fact]
        public void ReadByteArray()
        {
            byte[] value = null;

            byte[] testValue = null;

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(ByteArrayValue.Length);
                    binaryWriter.Write(ByteArrayValue);
                }

                testValue = memoryStream.ToArray();
            }

            using (INetPacketStream packetStream = new NetPacketStream(testValue))
                value = packetStream.Read<byte[]>();

            Assert.Equal(ByteArrayValue, value);
        }

        [Fact]
        public void WriteByte()
        {
            this.TestWrite<byte>(ByteTestArray, ByteValue);
        }

        [Fact]
        public void WriteShort()
        {
            this.TestWrite<short>(ShortTestArray, ShortValue);
        }

        [Fact]
        public void WriteInt32()
        {
            this.TestWrite<int>(Int32TestArray, Int32Value);
        }

        [Fact]
        public void WriteLong()
        {
            this.TestWrite<long>(Int64TestArray, Int64Value);
        }

        [Fact]
        public void WriteString()
        {
            string readString = null;
            byte[] packetStreamBuffer = null;

            using (INetPacketStream packetStream = new NetPacketStream())
            {
                packetStream.Write(StringValue);
                packetStreamBuffer = packetStream.Buffer;
            }

            using (INetPacketStream readPacketStream = new NetPacketStream(packetStreamBuffer))
            {
                readString = readPacketStream.Read<string>();
            }

            Assert.Equal(StringValue, readString);
        }

        [Fact]
        public void WriteByteArray()
        {
            byte[] readByteArray = null;
            byte[] packetStreamBuffer = null;

            using (INetPacketStream packetStream = new NetPacketStream())
            {
                packetStream.Write(ByteArrayValue);
                packetStreamBuffer = packetStream.Buffer;
            }

            using (INetPacketStream readPacketStream = new NetPacketStream(packetStreamBuffer))
            {
                readByteArray = readPacketStream.Read<byte[]>();
            }

            Assert.Equal(ByteArrayValue, readByteArray);
        }

        private void TestRead<T>(byte[] input, T expected)
        {
            T value = default(T);

            using (INetPacketStream packetStream = new NetPacketStream(input))
                value = packetStream.Read<T>();

            Assert.Equal(expected, value);
        }

        private void TestWrite<T>(byte[] expected, T value)
        {
            byte[] packetStreamBuffer = null;

            using (INetPacketStream packetStream = new NetPacketStream())
            {
                packetStream.Write(value);
                packetStreamBuffer = packetStream.Buffer;
            }

            Assert.Equal(expected, packetStreamBuffer);
        }
    }
}

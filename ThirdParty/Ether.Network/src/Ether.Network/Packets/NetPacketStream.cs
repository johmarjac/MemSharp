using System;
using System.IO;
using System.Linq;

namespace Ether.Network.Packets
{
    /// <summary>
    /// Creates a Ether.Network packet steram.
    /// </summary>
    public class NetPacketStream : MemoryStream, INetPacketStream
    {
        private enum PacketStateType
        {
            Write,
            Read
        }

        private readonly PacketStateType _state;
        private readonly BinaryReader _memoryReader;
        private readonly BinaryWriter _memoryWriter;

        /// <inheritdoc />
        public int Size => (int)this.Length;

        /// <inheritdoc />
        public virtual byte[] Buffer => this.GetInternalBuffer();

        /// <inheritdoc />
        public new long Position => base.Position;

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in write-only mode.
        /// </summary>
        public NetPacketStream()
        {
            this._memoryWriter = new BinaryWriter(this);
            this._state = PacketStateType.Write;
        }

        /// <summary>
        /// Creates and initializes a new <see cref="NetPacketStream"/> instance in read-only mode.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        public NetPacketStream(byte[] buffer)
            : base(buffer)
        {
            this._memoryReader = new BinaryReader(this);
            this._state = PacketStateType.Read;
        }

        /// <inheritdoc />
        public virtual T Read<T>()
        {
            if (this._state != PacketStateType.Read)
                throw new InvalidOperationException("Packet is in write-only mode.");

            Type type = typeof(T);

            if (NetPacketMethods.ReadMethods.ContainsKey(type))
                return (T)NetPacketMethods.ReadMethods[type](this._memoryReader);

            return default(T);
        }

        /// <inheritdoc />
        public virtual T[] ReadArray<T>(int amount)
        {
            if (this._state != PacketStateType.Read)
                throw new InvalidOperationException("Packet is in write-only mode.");

            Type type = typeof(T);
            var array = new T[amount];

            if (type == typeof(byte))
                array = this._memoryReader.ReadBytes(amount) as T[];
            else if (NetPacketMethods.ReadMethods.ContainsKey(type))
            {
                for (var i = 0; i < amount; i++)
                    array[i] = this.Read<T>();
            }

            return array;
        }

        /// <inheritdoc />
        public virtual void Write<T>(T value)
        {
            if (this._state != PacketStateType.Write)
                throw new InvalidOperationException("Packet is in read-only mode.");

            Type type = typeof(T);

            if (NetPacketMethods.WriteMethods.ContainsKey(type))
                NetPacketMethods.WriteMethods[type](this._memoryWriter, value);
        }

        /// <summary>
        /// Get stream buffer.
        /// </summary>
        /// <returns></returns>
        private byte[] GetInternalBuffer()
        {
#if NET45 || NET451
            return base.GetBuffer();
#else
            return this.TryGetBuffer(out ArraySegment<byte> buffer) ? buffer.ToArray() : new byte[0];
#endif
        }
    }
}

namespace Ether.Network.Packets
{
    /// <summary>
    /// Defines the behavior of a <see cref="IPacketProcessor"/>.
    /// </summary>
    public interface IPacketProcessor
    {
        /// <summary>
        /// Gets the packet header size.
        /// </summary>
        int HeaderSize { get; }
        
        /// <summary>
        /// Gets a value indicating whether to put the packet header in front of the packet <see cref="INetPacketStream.Buffer"/>.
        /// </summary>
        bool IncludeHeader { get; }

        /// <summary>
        /// Gets the packet message length.
        /// </summary>
        /// <param name="buffer">Header buffer</param>
        /// <returns>Packet message data length</returns>
        int GetMessageLength(byte[] buffer);

        /// <summary>
        /// Creates a new <see cref="INetPacketStream"/> packet instance.
        /// </summary>
        /// <param name="buffer">Input buffer</param>
        /// <returns>New packet</returns>
        INetPacketStream CreatePacket(byte[] buffer);
    }
}

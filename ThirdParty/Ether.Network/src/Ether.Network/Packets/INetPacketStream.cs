using System;

namespace Ether.Network.Packets
{
    /// <summary>
    /// Ether.Network Packet stream interface.
    /// </summary>
    public interface INetPacketStream : IDisposable
    {
        /// <summary>
        /// Gets the size of the packet.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the current position of the cursor in the packet stream.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Gets the packet buffer.
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Reads a T value from the packet.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <returns>The value read and converted to the type.</returns>
        T Read<T>();

        /// <summary>
        /// Reads an array of T value from the packet.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="amount">Amount to read.</param>
        /// <returns>An array of type T.</returns>
        T[] ReadArray<T>(int amount);

        /// <summary>
        /// Writes a T value in the packet.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">Value to write in the packet stream.</param>
        void Write<T>(T value);
    }
}

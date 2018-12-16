using Ether.Network.Packets;
using System;

namespace Ether.Network.Client
{
    /// <summary>
    /// <see cref="INetClient"/> interface.
    /// </summary>
    public interface INetClient : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="INetClient"/> unique Id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the <see cref="INetClient"/> connected state.
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// Gets the <see cref="INetClient"/> running state.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Connects to a remote server.
        /// </summary>
        void Connect();
        
        /// <summary>
        /// Disconnects from the remote server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends packets to the remote server.
        /// </summary>
        /// <param name="packet"></param>
        void Send(INetPacketStream packet);
    }
}

using Ether.Network.Common;
using Ether.Network.Packets;
using System;
using System.Collections.Generic;

namespace Ether.Network.Server
{
    /// <summary>
    /// Provides a simple and scalable TCP server.
    /// </summary>
    public interface INetServer : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="INetServer"/> running state.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the server.
        /// </summary>
        void Stop();

        /// <summary>
        /// Disconnects a client from the server.
        /// </summary>
        /// <param name="clientId">Client unique id</param>
        void DisconnectClient(Guid clientId);

        /// <summary>
        /// Sends a packet to a list of connected users.
        /// </summary>
        /// <param name="users">List of users.</param>
        /// <param name="packet">Packet to send.</param>
        void SendTo(IEnumerable<INetUser> users, INetPacketStream packet);

        /// <summary>
        /// Sends a packet to every connected user.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        void SendToAll(INetPacketStream packet);

        /// <summary>
        /// Gets the user by his Id.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User</returns>
        INetUser GetUser(Guid id);
    }
}

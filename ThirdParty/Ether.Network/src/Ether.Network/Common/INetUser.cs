using Ether.Network.Packets;
using Ether.Network.Server;
using System.Collections.Generic;

namespace Ether.Network.Common
{
    /// <summary>
    /// Defines the behavior of an Ether.Network connected user.
    /// </summary>
    public interface INetUser : INetConnection
    {
        /// <summary>
        /// Gets the <see cref="INetServer"/> instance the <see cref="INetUser"/> is connected to.
        /// </summary>
        INetServer Server { get; }

        /// <summary>
        /// Sends a packet throught the network.
        /// </summary>
        /// <param name="packet">Outgoing packet</param>
        void Send(INetPacketStream packet);

        /// <summary>
        /// Sends a packet to a list of users.
        /// </summary>
        /// <param name="users">List of users</param>
        /// <param name="packet">Outgoing packet</param>
        void SendTo(IEnumerable<INetUser> users, INetPacketStream packet);

        /// <summary>
        /// Sends a packet to all users on the server.
        /// </summary>
        /// <param name="packet">Outgoing packet</param>
        void SendToAll(INetPacketStream packet);

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        void HandleMessage(INetPacketStream packet);
    }
}

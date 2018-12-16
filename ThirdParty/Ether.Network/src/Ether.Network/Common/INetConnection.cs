using System;
using System.Net.Sockets;

namespace Ether.Network.Common
{
    /// <summary>
    /// Describes the behavior of a Ether.Network connection
    /// </summary>
    public interface INetConnection : IDisposable
    {
        /// <summary>
        /// Gets the connection's unique id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the connection's socket.
        /// </summary>
        Socket Socket { get; }
    }
}

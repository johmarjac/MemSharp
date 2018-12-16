using System;
using System.Net.Sockets;

namespace Ether.Network.Common.Data
{
    /// <summary>
    /// Defines the user's async token that contains all informations about data receiving process.
    /// </summary>
    internal interface IAsyncUserToken
    {
        /// <summary>
        /// Gets or sets the number of bytes received of the Header.
        /// </summary>
        int ReceivedHeaderBytesCount { get; set; }

        /// <summary>
        /// Gets or sets the received header data.
        /// </summary>
        byte[] HeaderData { get; set; }

        /// <summary>
        /// Gets or sets the full message size.
        /// </summary>
        int? MessageSize { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes received of the Message.
        /// </summary>
        int ReceivedMessageBytesCount { get; set; }

        /// <summary>
        /// Gets or sets the received message data.
        /// </summary>
        byte[] MessageData { get; set; }

        /// <summary>
        /// Gets or sets the data start offset.
        /// </summary>
        int DataStartOffset { get; set; }

        /// <summary>
        /// Gets or sets the user's socket.
        /// </summary>
        Socket Socket { get; set; }

        /// <summary>
        /// Gets or sets the action to invoke when we receive a message.
        /// </summary>
        Action<byte[]> MessageHandler { get; set; }

        /// <summary>
        /// Resets the <see cref="IAsyncUserToken"/>.
        /// </summary>
        void Reset();
    }
}

using System;
using System.Net.Sockets;

namespace Ether.Network.Common.Data
{
    /// <inheritdoc />
    internal sealed class AsyncUserToken : IAsyncUserToken
    {
        /// <inheritdoc />
        public int ReceivedHeaderBytesCount { get; set; }

        /// <inheritdoc />
        public byte[] HeaderData { get; set; }

        /// <inheritdoc />
        public int? MessageSize { get; set; }

        /// <inheritdoc />
        public int ReceivedMessageBytesCount { get; set; }

        /// <inheritdoc />
        public byte[] MessageData { get; set; }

        /// <inheritdoc />
        public int DataStartOffset { get; set; }

        /// <inheritdoc />
        public Socket Socket { get; set; }

        /// <inheritdoc />
        public Action<byte[]> MessageHandler { get; set; }

        /// <summary>
        /// Creates a new <see cref="AsyncUserToken"/> instance.
        /// </summary>
        public AsyncUserToken()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="AsyncUserToken"/> instance.
        /// </summary>
        /// <param name="messageHandlerAction">Action to execute when a message is received.</param>
        private AsyncUserToken(Action<byte[]> messageHandlerAction)
        {
            this.ReceivedHeaderBytesCount = 0;
            this.ReceivedMessageBytesCount = 0;
            this.HeaderData = null;
            this.MessageSize = null;
            this.MessageData = null;
            this.DataStartOffset = 0;
            this.MessageHandler = messageHandlerAction;
        }

        /// <inheritdoc />
        public void Reset()
        {
            this.ReceivedHeaderBytesCount = 0;
            this.ReceivedMessageBytesCount = 0;
            this.HeaderData = null;
            this.MessageData = null;
            this.MessageSize = null;
        }
    }
}
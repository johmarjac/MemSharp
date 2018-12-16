using Ether.Network.Common;
using System;

namespace Ether.Network.Common.Data
{
    /// <summary>
    /// Defines a structure that handles an incoming message.
    /// </summary>
    internal struct MessageData : IEquatable<MessageData>
    {
        /// <summary>
        /// Gets the owner of the incoming message.
        /// </summary>
        public INetUser User { get; }

        /// <summary>
        /// Gets the incoming message data.
        /// </summary>
        public byte[] Message { get; }

        /// <summary>
        /// Creates a new <see cref="MessageData"/> instance.
        /// </summary>
        /// <param name="user">Message owner</param>
        /// <param name="message">Message data</param>
        public MessageData(INetUser user, byte[] message)
        {
            this.User = user;
            this.Message = message;
        }

        /// <summary>
        /// Compares two <see cref="MessageData"/> objects.
        /// </summary>
        /// <param name="other">Other <see cref="MessageData"/></param>
        /// <returns>True if equal, false otherwise</returns>
        public bool Equals(MessageData other)
        {
            return this.User.Id == other.User.Id
                && this.Message == other.Message;
        }
    }
}

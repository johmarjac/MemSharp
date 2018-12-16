using System;

namespace Ether.Network.Exceptions
{
    /// <summary>
    /// Represents an Ether.Network packet exception.
    /// </summary>
    public class EtherPacketException : EtherException
    {
        /// <summary>
        /// Creates a new <see cref="EtherPacketException"/>.
        /// </summary>
        /// <param name="message"></param>
        public EtherPacketException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherPacketException"/> with an inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EtherPacketException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

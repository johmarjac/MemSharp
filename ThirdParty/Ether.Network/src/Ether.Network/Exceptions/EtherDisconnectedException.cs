using System;

namespace Ether.Network.Exceptions
{
    /// <summary>
    /// Represents an Ether.Network disconnected exception.
    /// </summary>
    /// <remarks>
    /// This exception is thrown when a client is disconnected from the server.
    /// </remarks>
    public class EtherDisconnectedException : EtherException
    {
        /// <summary>
        /// Creates a new <see cref="EtherDisconnectedException"/> instance.
        /// </summary>
        public EtherDisconnectedException()
            : this("Disconnected")
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherDisconnectedException"/> instance.
        /// </summary>
        /// <param name="message"></param>
        public EtherDisconnectedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherDisconnectedException"/> instance.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EtherDisconnectedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}

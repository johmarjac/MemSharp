using System;

namespace Ether.Network.Exceptions
{
    /// <summary>
    /// Represents an Ether.Network client not found exception.
    /// </summary>
    public class EtherClientNotFoundException : EtherException
    {
        /// <summary>
        /// Creates a new <see cref="EtherClientNotFoundException"/>.
        /// </summary>
        /// <param name="clientId">Client Unique Id</param>
        public EtherClientNotFoundException(Guid clientId)
            : base($"Cannot found client {clientId}.")
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherClientNotFoundException"/>.
        /// </summary>
        /// <param name="message"></param>
        public EtherClientNotFoundException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherClientNotFoundException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EtherClientNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}

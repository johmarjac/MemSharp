using System;

namespace Ether.Network.Exceptions
{
    /// <summary>
    /// Represents a generic Ether.Network exception.
    /// </summary>
    public class EtherException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="EtherException"/>.
        /// </summary>
        /// <param name="message"></param>
        public EtherException(string message)
            : base(message, null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EtherException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

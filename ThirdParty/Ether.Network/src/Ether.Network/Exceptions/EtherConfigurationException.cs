namespace Ether.Network.Exceptions
{
    /// <summary>
    /// Represents an Ether.Network configuration exception.
    /// </summary>
    public class EtherConfigurationException : EtherException
    {
        /// <summary>
        /// Creates a new <see cref="EtherConfigurationException"/>.
        /// </summary>
        public EtherConfigurationException()
            : base(string.Empty)
        {
        }

        /// <summary>
        /// Creates a new <see cref="EtherConfigurationException"/>.
        /// </summary>
        /// <param name="message"></param>
        public EtherConfigurationException(string message) 
            : base(message)
        {
        }
    }
}

using System.Net;
using System.Linq;
using System;
using System.Net.Sockets;
using Ether.Network.Exceptions;

namespace Ether.Network.Utils
{
    /// <summary>
    /// Provides utility methods.
    /// </summary>
    internal static class NetUtils
    {
        /// <summary>
        /// Gets an <see cref="IPAddress"/> from an IP or a host string.
        /// </summary>
        /// <param name="ipOrHost">IP or Host address.</param>
        /// <returns>Parsed <see cref="IPAddress"/>.</returns>
        public static IPAddress GetIpAddress(string ipOrHost)
        {
            if (IPAddress.TryParse(ipOrHost, out IPAddress address))
            {
                return address;
            }
            else
            {
                return Dns.GetHostAddressesAsync(ipOrHost).Result
                    .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Creates a new <see cref="IPEndPoint"/> with an IP or host and a port number.
        /// </summary>
        /// <param name="ipOrHost">IP or Host address.</param>
        /// <param name="port">Port number.</param>
        /// <returns></returns>
        public static IPEndPoint CreateIpEndPoint(string ipOrHost, int port)
        {
            IPAddress address = GetIpAddress(ipOrHost);

            if (address == null)
                throw new EtherConfigurationException($"Invalid host or ip address: {ipOrHost}.");
            if (port <= 0)
                throw new EtherConfigurationException($"Invalid port: {port}");

            return new IPEndPoint(address, port);
        }

        /// <summary>
        /// Gets the buffer at the offset and size passed.
        /// </summary>
        /// <param name="bufferSource">Input buffer source</param>
        /// <param name="offset">Data offset</param>
        /// <param name="size">Data size</param>
        /// <returns></returns>
        public static byte[] GetPacketBuffer(byte[] bufferSource, int offset, int size)
        {
            var buffer = new byte[size];

            Buffer.BlockCopy(bufferSource, offset, buffer, 0, size);

            return buffer;
        }

        /// <summary>
        /// Creates a new <see cref="SocketAsyncEventArgs"/> instance.
        /// </summary>
        /// <param name="userToken">User token</param>
        /// <param name="completedAction">Completed operation action</param>
        /// <returns></returns>
        public static SocketAsyncEventArgs CreateSocketAsync(object userToken, EventHandler<SocketAsyncEventArgs> completedAction)
        {
            var socketAsync = new SocketAsyncEventArgs
            {
                UserToken = userToken
            };

            socketAsync.Completed += completedAction;

            return socketAsync;
        }

        /// <summary>
        /// Creates a new <see cref="SocketAsyncEventArgs"/> instance.
        /// </summary>
        /// <param name="userToken">User token</param>
        /// <param name="completedAction">Completed operation action</param>
        /// <param name="bufferSize">Buffer size</param>
        /// <returns></returns>
        public static SocketAsyncEventArgs CreateSocketAsync(object userToken, EventHandler<SocketAsyncEventArgs> completedAction, int bufferSize)
        {
            SocketAsyncEventArgs socketAsync = CreateSocketAsync(userToken, completedAction);

            if (bufferSize > 0)
                socketAsync.SetBuffer(new byte[bufferSize], 0, bufferSize);

            return socketAsync;
        }
    }
}

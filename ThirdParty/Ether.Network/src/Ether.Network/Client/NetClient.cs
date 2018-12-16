using Ether.Network.Common;
using Ether.Network.Common.Data;
using Ether.Network.Packets;
using Ether.Network.Utils;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ether.Network.Exceptions;

namespace Ether.Network.Client
{
    /// <summary>
    /// Managed TCP client.
    /// </summary>
    public abstract class NetClient : NetUser, INetClient
    {
        private static readonly IPacketProcessor DefaultPacketProcessor = new NetPacketProcessor();
        
        private readonly AutoResetEvent _autoConnectEvent;
        private readonly AutoResetEvent _autoSendEvent;
        private readonly BlockingCollection<byte[]> _sendingQueue;
        private readonly BlockingCollection<byte[]> _receivingQueue;
        private readonly Task _sendingQueueWorker;
        private readonly Task _receivingQueueWorker;
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly CancellationToken _cancelToken;

        private bool _isDisposed;
        private SocketAsyncEventArgs _socketReceiveArgs;
        private SocketAsyncEventArgs _socketSendArgs;

        /// <summary>
        /// Gets the packet processor.
        /// </summary>
        protected virtual IPacketProcessor PacketProcessor => DefaultPacketProcessor;

        /// <inheritdoc />
        public bool IsConnected => this.Socket != null && this.Socket.Connected;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the <see cref="NetClient"/> configuration.
        /// </summary>
        protected NetClientConfiguration Configuration { get; }

        /// <summary>
        /// Creates a new <see cref="NetClient"/> instance.
        /// </summary>
        protected NetClient()
        {
            this.Configuration = new NetClientConfiguration(this);
            this._autoConnectEvent = new AutoResetEvent(false);
            this._autoSendEvent = new AutoResetEvent(false);
            this._sendingQueue = new BlockingCollection<byte[]>();
            this._receivingQueue = new BlockingCollection<byte[]>();
            this._cancelTokenSource = new CancellationTokenSource();
            this._cancelToken = this._cancelTokenSource.Token;
            this._sendingQueueWorker = new Task(this.ProcessSendingQueue, this._cancelToken);
            this._receivingQueueWorker = new Task(this.ProcessReceiveQueue, this._cancelToken);
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Client is already running");

            if (this.IsConnected)
                throw new InvalidOperationException("Client is already connected to remote.");

            this.CheckConfiguration();

            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socketSendArgs = NetUtils.CreateSocketAsync(this.Socket, this.IO_Completed);
            this._socketReceiveArgs = NetUtils.CreateSocketAsync(this, this.IO_Completed, this.Configuration.BufferSize);

            SocketAsyncEventArgs connectSocket = NetUtils.CreateSocketAsync(this.Socket, this.IO_Completed);
            connectSocket.RemoteEndPoint = NetUtils.CreateIpEndPoint(this.Configuration.Host, this.Configuration.Port);

            SocketError errorCode = ConnectSocketToServer(connectSocket);
            
            if (!IsConnected)
            {
                if (this.Configuration.RetryMode == NetClientRetryOptions.Limited)
                {
                    int count = 0;
                    
                    while (!IsConnected && count < this.Configuration.MaxRetryAttempts)
                    {
                        errorCode = ConnectSocketToServer(connectSocket);
                        count++;
                    }
                }
                else if (this.Configuration.RetryMode == NetClientRetryOptions.Infinite)
                {
                    while (!IsConnected)
                    {
                        errorCode = ConnectSocketToServer(connectSocket);
                    }
                }
                
                if (!IsConnected)
                {
                    this.OnSocketError(errorCode);
                    return;
                }
            }

            this._sendingQueueWorker.Start();
            this._receivingQueueWorker.Start();
            this.Token.Socket = this.Socket;
            this.Token.MessageHandler = data => this._receivingQueue.Add(data, this._cancelToken);
            this.IsRunning = true;

            if (!this.Socket.ReceiveAsync(this._socketReceiveArgs))
                this.ProcessReceive(this._socketReceiveArgs);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (this.IsConnected)
            {
#if !NETSTANDARD1_3
                this.Socket.Close();
#else
                this.Socket.Shutdown(SocketShutdown.Both);
#endif
                this.Socket.Dispose();
                this.Socket = null;
            }

            this._socketSendArgs?.Dispose();
            this._socketSendArgs = null;
            this._socketReceiveArgs?.Dispose();
            this._socketReceiveArgs = null;

            this.IsRunning = false;
            this._cancelTokenSource.Cancel(false);
            this.OnDisconnected();
        }

        /// <inheritdoc />
        public override void Send(INetPacketStream packet)
        {
            if (!this.IsConnected)
                throw new SocketException();

            this._sendingQueue.Add(packet.Buffer, this._cancelToken);
        }

        /// <summary>
        /// Triggered when the <see cref="NetClient"/> receives a packet.
        /// </summary>
        /// <param name="packet"></param>
        public override void HandleMessage(INetPacketStream packet)
        {
            // Nothing to handle
        }

        /// <summary>
        /// Triggered when the client is connected to the remote end point.
        /// </summary>
        protected abstract void OnConnected();

        /// <summary>
        /// Triggered when the client is disconnected from the remote end point.
        /// </summary>
        protected abstract void OnDisconnected();

        /// <summary>
        /// Triggered when a error on the socket happend
        /// </summary>
        /// <param name="socketError"></param>
        protected abstract void OnSocketError(SocketError socketError);

        /// <summary>
        /// Sends the packets in the sending queue.
        /// </summary>
        private void ProcessSendingQueue()
        {
            while (true)
            {
                try
                {
                    byte[] packetBuffer = this._sendingQueue.Take(this._cancelToken);

                    if (packetBuffer == null || packetBuffer.Length <= 0)
                        continue;

                    this._socketSendArgs.SetBuffer(packetBuffer, 0, packetBuffer.Length);

                    if (this.Socket.SendAsync(this._socketSendArgs))
                        this._autoSendEvent.WaitOne();
                }
                catch (Exception e)
                {
                    if ((e is AggregateException || e is OperationCanceledException) && this._cancelTokenSource.IsCancellationRequested)
                        break;
                }
            }
        }

        /// <summary>
        /// Process and dispatch the received packets.
        /// </summary>
        private void ProcessReceiveQueue()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = this._receivingQueue.Take(this._cancelToken);

                    if (buffer == null)
                        continue;
                    
                    Task.Run(() =>
                    {
                        using (INetPacketStream packet = this.PacketProcessor.CreatePacket(buffer))
                            this.HandleMessage(packet);
                    }, this._cancelToken);
                }
                catch (Exception e)
                {
                    if ((e is AggregateException || e is OperationCanceledException) && this._cancelTokenSource.IsCancellationRequested)
                        break;
                }
            }
        }

        /// <summary>
        /// Process receieve.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (e.BytesTransferred <= 0 || !(e.UserToken is NetUser user))
                    return;
                
                SocketAsyncUtils.ReceiveData(e, user.Token, this.PacketProcessor);

                if (!user.Socket.ReceiveAsync(e))
                    this.ProcessReceive(e);
            }
            else if (e.SocketError == SocketError.ConnectionReset)
            {
                this.Disconnect();
            }
        }

        /// <summary>
        /// Checks the configuration.
        /// </summary>
        private void CheckConfiguration()
        {
            if (this.Configuration.Port <= 0)
                throw new EtherConfigurationException($"{this.Configuration.Port} is not a valid port.");
            
            if (this.Configuration.Address == null)
                throw new EtherConfigurationException($"Invalid host : {this.Configuration.Host}.");

            if (this.Configuration.BufferSize <= 0)
                throw new EtherConfigurationException("BufferSize cannot less or equal to 0.");
        }

        /// <summary>
        /// Triggered when a <see cref="SocketAsyncEventArgs"/> async operation is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (e.SocketError == SocketError.ConnectionReset)
            {
                this.Disconnect();
                return;
            }

            if (e.SocketError != SocketError.Success)
                return;

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this.OnConnected();
                    this._autoConnectEvent.Set();
                    break;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this._autoSendEvent.Set();
                    break;
                case SocketAsyncOperation.Disconnect:
                    this.Disconnect();
                    break;
                default: throw new InvalidOperationException("Unexpected socket async operation.");
            }
        }

        /// <summary>
        /// Dispose the <see cref="NetClient"/> instance.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                this._autoConnectEvent.Dispose();
                this._autoSendEvent.Dispose();
                this.Disconnect();
            }

            this._isDisposed = true;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Connects a socket to a server
        /// </summary>
        /// <param name="connectSocket">The socket to connect</param>
        /// <returns>The socket error</returns>
        private SocketError ConnectSocketToServer(SocketAsyncEventArgs connectSocket)
        {
            if (this.Socket.ConnectAsync(connectSocket))
                this._autoConnectEvent.WaitOne(Configuration.TimeOut);

            return connectSocket.SocketError;
        }
    }
}
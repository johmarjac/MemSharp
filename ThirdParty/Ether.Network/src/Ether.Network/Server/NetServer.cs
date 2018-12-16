using Ether.Network.Common;
using Ether.Network.Common.Data;
using Ether.Network.Exceptions;
using Ether.Network.Extensions;
using Ether.Network.Packets;
using Ether.Network.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ether.Network.Server
{
    /// <inheritdoc />
    public abstract class NetServer<T> : NetConnection, INetServer where T : NetUser, new()
    {
        private static readonly IPacketProcessor DefaultPacketProcessor = new NetPacketProcessor();
        private const string AllInterfaces = "0.0.0.0";

        private readonly ManualResetEvent _manualResetEvent;
        private readonly AutoResetEvent _autoSendEvent;
        private readonly ConcurrentDictionary<Guid, T> _clients;
        private readonly BlockingCollection<MessageData> _messageQueue;
        private readonly SocketAsyncEventArgsPool _readPool;
        private readonly SocketAsyncEventArgsPool _writePool;
        private readonly CancellationTokenSource _sendQueueTaskCancelTokenSource;
        private readonly CancellationToken _sendQueueCancelToken;

        private bool _isDisposed;

        /// <summary>
        /// Gets the <see cref="NetServer{T}"/> configuration
        /// </summary>
        protected NetServerConfiguration Configuration { get; }

        /// <summary>
        /// Gets the packet processor.
        /// </summary>
        protected virtual IPacketProcessor PacketProcessor => DefaultPacketProcessor;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the connected client.
        /// </summary>
        public IEnumerable<T> Clients => this._clients.Values;

        /// <summary>
        /// Creates a new <see cref="NetServer{T}"/> instance.
        /// </summary>
        protected NetServer()
        {
            this.Configuration = new NetServerConfiguration(this);
            this._clients = new ConcurrentDictionary<Guid, T>();
            this._messageQueue = new BlockingCollection<MessageData>();
            this._readPool = new SocketAsyncEventArgsPool();
            this._writePool = new SocketAsyncEventArgsPool();

            this._manualResetEvent = new ManualResetEvent(false);
            this._autoSendEvent = new AutoResetEvent(false);

            this._sendQueueTaskCancelTokenSource = new CancellationTokenSource();
            this._sendQueueCancelToken = this._sendQueueTaskCancelTokenSource.Token;
            Task.Factory.StartNew(this.ProcessSendQueue, this._sendQueueCancelToken);
        }

        /// <inheritdoc />
        public void Start()
        {
            if (this.IsRunning)
                throw new InvalidOperationException("Server is already running.");

            this.CheckConfiguration();

            for (var i = 0; i < this.Configuration.MaximumNumberOfConnections; i++)
            {
                this._readPool.Push(NetUtils.CreateSocketAsync(null, this.IO_Completed, this.Configuration.BufferSize));
                this._writePool.Push(NetUtils.CreateSocketAsync(null, this.IO_Completed, this.Configuration.BufferSize));
            }

            this.Initialize();
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.Socket.Bind(NetUtils.CreateIpEndPoint(this.Configuration.Host, this.Configuration.Port));
            this.Socket.Listen(this.Configuration.Backlog);
            this.IsRunning = true;
            this.StartAccept(NetUtils.CreateSocketAsync(null, this.IO_Completed));

            if (this.Configuration.Blocking)
                this._manualResetEvent.WaitOne();
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (!this.IsRunning)
                return;

            this.ClearResources();

            if (this.Socket != null)
            {
#if !NETSTANDARD1_3
                this.Socket.Close();
#endif
                this.Socket.Dispose();
                this.Socket = null;
            }

            if (this.Configuration.Blocking)
                this._manualResetEvent.Set();

            this.IsRunning = false;
        }

        /// <inheritdoc />
        public void DisconnectClient(Guid clientId)
        {
            if (!this._clients.TryRemove(clientId, out T removedClient))
                return;

            this.OnClientDisconnected(removedClient);
            removedClient.Dispose();
        }

        /// <inheritdoc />
        public void SendTo(IEnumerable<INetUser> users, INetPacketStream packet)
        {
            foreach (INetUser user in users)
            {
                user.Send(packet);
            }
        }

        /// <inheritdoc />
        public void SendToAll(INetPacketStream packet) => this.SendTo(this.Clients, packet);

        /// <inheritdoc />
        public INetUser GetUser(Guid id) => this._clients.TryGetValue(id, out T user) ? user : null;

        /// <summary>
        /// Initialize the server resourrces.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Triggered when a new client is connected to the server.
        /// </summary>
        /// <param name="connection">Connected client</param>
        protected abstract void OnClientConnected(T connection);

        /// <summary>
        /// Triggered when a client disconnects from the server.
        /// </summary>
        /// <param name="connection">Disconnected client</param>
        protected abstract void OnClientDisconnected(T connection);

        /// <summary>
        /// Triggered when an error occurs on the server.
        /// </summary>
        /// <param name="exception">Exception</param>
        protected abstract void OnError(Exception exception);

        /// <summary>
        /// Starts the accept connection async operation.
        /// </summary>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e.AcceptSocket != null)
                e.AcceptSocket = null;

            if (!this.IsRunning)
                return;

            if (!this.Socket.AcceptAsync(e))
                this.ProcessAccept(e);
        }

        /// <summary>
        /// Process the accept connection async operation.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    SocketAsyncEventArgs readArgs = this._readPool.Pop();

                    if (readArgs == null)
                        return;

                    var client = new T
                    {
                        Socket = e.AcceptSocket,
                        Server = this,
                        SendAction = this.SendMessageAction
                    };
                    client.Token.Socket = client.Socket;
                    client.Token.MessageHandler = messageData => this.HandleIncomingMessages(client, messageData);

                    if (!this._clients.TryAdd(client.Id, client))
                        throw new EtherException($"Client {client.Id} already exists in client list.");

                    this.OnClientConnected(client);
                    readArgs.UserToken = client;

                    if (!e.AcceptSocket.ReceiveAsync(readArgs))
                        this.ProcessReceive(readArgs);

                    this.StartAccept(e);
                }
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }
        }

        /// <summary>
        /// Process the send async operation.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            this._writePool.Push(e);
            this._autoSendEvent.Set();
        }

        /// <summary>
        /// Adds the message to the sending queue.
        /// </summary>
        /// <param name="user">User that sent the message</param>
        /// <param name="message">Message</param>
        private void SendMessageAction(INetUser user, byte[] message)
        {
            if (this._clients.ContainsKey(user.Id))
            {
                this._messageQueue.Add(new MessageData(user, message));
            }
        }

        /// <summary>
        /// Process the send queue.
        /// </summary>
        private void ProcessSendQueue()
        {
            while (true)
            {
                try
                {
                    MessageData message = this._messageQueue.Take(this._sendQueueCancelToken);

                    if (message.User != null && message.Message != null)
                        this.SendMessage(message);
                }
                catch (Exception e)
                {
                    this.OnError(e);
                    if (this._sendQueueCancelToken.IsCancellationRequested)
                        break;
                }
            }
        }

        /// <summary>
        /// Sends the message through the network.
        /// </summary>
        /// <param name="messageData"></param>
        private void SendMessage(MessageData messageData)
        {
            SocketAsyncEventArgs writeEventArgs = this._writePool.Pop();

            if (writeEventArgs != null)
            {
                writeEventArgs.SetBuffer(messageData.Message, 0, messageData.Message.Length);
                writeEventArgs.UserToken = messageData.User;

                if (!messageData.User.Socket.SendAsync(writeEventArgs))
                    this.ProcessSend(writeEventArgs);
            }
            else
            {
                this._autoSendEvent.WaitOne();
                this.SendMessage(messageData);
            }
        }

        /// <summary>
        /// Process receieve.
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                if (!(e.UserToken is NetUser connection))
                    return;
                
                SocketAsyncUtils.ReceiveData(e, connection.Token, this.PacketProcessor);

                if (!connection.Socket.ReceiveAsync(e))
                    this.ProcessReceive(e);
            }
            else
            {
                this.CloseConnection(e);
            }
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="e"></param>
        private void CloseConnection(SocketAsyncEventArgs e)
        {
            if (!(e.UserToken is INetUser connection))
                return;

            if (!this.IsRunning)
                return;

            this._readPool.Push(e);
            this.DisconnectClient(connection.Id);
        }

        /// <summary>
        /// Handle incoming message packets.
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="messageData">Incoming message data</param>
        private void HandleIncomingMessages(T user, byte[] messageData)
        {
            Task.Run(() =>
            {
                using (INetPacketStream packet = this.PacketProcessor.CreatePacket(messageData))
                    user.HandleMessage(packet);
            });
        }

        /// <summary>
        /// Clear NetServer's resources.
        /// </summary>
        private void ClearResources()
        {
            foreach (T client in this.Clients)
                client.Dispose();

            this._clients.Clear();
            this._readPool.Clear();
            this._writePool.Clear();
            this._messageQueue.Clear();
        }

        /// <summary>
        /// Checks the configuration.
        /// </summary>
        private void CheckConfiguration()
        {
            if (this.Configuration.Port <= 0)
                throw new EtherConfigurationException($"{this.Configuration.Port} is not a valid port.");

            IPAddress address = this.Configuration.Host == AllInterfaces ? IPAddress.Any : this.Configuration.Address;
            if (address == null)
                throw new EtherConfigurationException($"Invalid host : {this.Configuration.Host}.");

            if (this.Configuration.BufferSize <= 0)
                throw new EtherConfigurationException("BufferSize cannot less or equal to 0.");

            if (this.Configuration.Backlog <= 0)
                throw new EtherConfigurationException("Backlog cannot be less or equal to 0.");
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

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.ProcessAccept(e);
                    break;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    this.ProcessSend(e);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected SocketAsyncOperation.");
            }
        }

        /// <summary>
        /// Dispose the <see cref="NetServer{T}"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._sendQueueTaskCancelTokenSource.Cancel(false);
                    this.ClearResources();
                    this._readPool.Dispose();
                    this._writePool.Dispose();
                    this._messageQueue.Dispose();
                    
                    this._isDisposed = true;
                }
            }

            base.Dispose(disposing);
        }
    }
}
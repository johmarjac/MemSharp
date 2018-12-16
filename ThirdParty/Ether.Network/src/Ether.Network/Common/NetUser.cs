using Ether.Network.Common.Data;
using Ether.Network.Packets;
using Ether.Network.Server;
using System;
using System.Collections.Generic;

namespace Ether.Network.Common
{
    /// <inheritdoc />
    public abstract class NetUser : NetConnection, INetUser
    {
        /// <inheritdoc />
        public INetServer Server { get; internal set; }

        /// <summary>
        /// Gets the user token.
        /// </summary>
        internal IAsyncUserToken Token { get; }

        /// <summary>
        /// Gets or sets the send packet action.
        /// </summary>
        internal Action<INetUser, byte[]> SendAction { private get; set; }

        /// <summary>
        /// Creates a new <see cref="NetUser"/> instance.
        /// </summary>
        protected NetUser()
        {
            this.Server = null;
            this.Token = new AsyncUserToken();
        }

        /// <inheritdoc />
        public virtual void HandleMessage(INetPacketStream packet)
        {
            // Nothing to do.
        }

        /// <inheritdoc />
        public virtual void Send(INetPacketStream packet) => this.SendAction?.Invoke(this, packet.Buffer);

        /// <inheritdoc />
        public void SendTo(IEnumerable<INetUser> users, INetPacketStream packet) => this.Server?.SendTo(users, packet);

        /// <inheritdoc />
        public void SendToAll(INetPacketStream packet) => this.Server?.SendToAll(packet);
    }
}

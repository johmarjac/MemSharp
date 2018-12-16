using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Ether.Network.Utils
{
    /// <summary>
    /// Represents an object pool of <see cref="SocketAsyncEventArgs"/> elements.
    /// </summary>
    internal sealed class SocketAsyncEventArgsPool : IDisposable
    {
        private bool _disposedValue;
        private readonly ConcurrentStack<SocketAsyncEventArgs> _socketPool;

        /// <summary>
        /// Creates a new <see cref="SocketAsyncEventArgsPool"/> instance with a maximal capacity.
        /// </summary>
        public SocketAsyncEventArgsPool()
        {
            this._socketPool = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        /// <summary>
        /// Destructs the <see cref="SocketAsyncEventArgsPool"/> instance.
        /// </summary>
        ~SocketAsyncEventArgsPool()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Pops a <see cref="SocketAsyncEventArgs"/> of the top of the stack.
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs Pop()
        {
            return this._socketPool.TryPop(out SocketAsyncEventArgs socketAsyncEventArgs) ? socketAsyncEventArgs : null;
        }

        /// <summary>
        /// Push a <see cref="SocketAsyncEventArgs"/> to the top of the stack.
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        public void Push(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs == null)
                throw new ArgumentNullException(nameof(socketAsyncEventArgs));

            Array.Clear(socketAsyncEventArgs.Buffer, 0, socketAsyncEventArgs.Buffer.Length);
            socketAsyncEventArgs.SetBuffer(socketAsyncEventArgs.Buffer, 0, socketAsyncEventArgs.Buffer.Length);
            this._socketPool.Push(socketAsyncEventArgs);
        }

        /// <summary>
        /// Clear the pool.
        /// </summary>
        public void Clear()
        {
            foreach (SocketAsyncEventArgs e in this._socketPool)
                e.Dispose();

            this._socketPool.Clear();
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (this._disposedValue)
                return;

            if (disposing)
            {
                this.Clear();
            }

            this._disposedValue = true;
        }
        
        /// <summary>
        /// Dispose the <see cref="SocketAsyncEventArgsPool"/> resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

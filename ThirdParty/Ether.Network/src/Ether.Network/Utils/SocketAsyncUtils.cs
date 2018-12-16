using Ether.Network.Common.Data;
using Ether.Network.Packets;
using System;
using System.Linq;
using System.Net.Sockets;

namespace Ether.Network.Utils
{
    internal static class SocketAsyncUtils
    {
        internal static void ReceiveData(SocketAsyncEventArgs e, IAsyncUserToken token, IPacketProcessor packetProcessor)
        {
            if (e == null)
                throw new ArgumentException(nameof(e));

            var receivedBuffer = new byte[e.BytesTransferred];
            Buffer.BlockCopy(e.Buffer, 0, receivedBuffer, 0, e.BytesTransferred);

            while (token.DataStartOffset < receivedBuffer.Length)
            {
                if (token.ReceivedHeaderBytesCount < packetProcessor.HeaderSize)
                {
                    if (token.HeaderData == null)
                        token.HeaderData = new byte[packetProcessor.HeaderSize];

                    int bufferRemainingBytes = receivedBuffer.Length - token.DataStartOffset;
                    int headerRemainingBytes = packetProcessor.HeaderSize - token.ReceivedHeaderBytesCount;
                    int bytesToRead = Math.Min(bufferRemainingBytes, headerRemainingBytes);
                    Buffer.BlockCopy(receivedBuffer, token.DataStartOffset, token.HeaderData, token.ReceivedHeaderBytesCount, bytesToRead);
                    token.ReceivedHeaderBytesCount += bytesToRead;
                    token.DataStartOffset += bytesToRead;
                }

                if (token.ReceivedHeaderBytesCount == packetProcessor.HeaderSize && token.HeaderData != null)
                {
                    if (!token.MessageSize.HasValue)
                        token.MessageSize = packetProcessor.GetMessageLength(token.HeaderData);
                    if (token.MessageSize.Value < 0)
                        throw new InvalidOperationException("Message size cannot be smaller than zero.");

                    if (token.MessageData == null)
                        token.MessageData = new byte[token.MessageSize.Value];

                    if (token.ReceivedMessageBytesCount < token.MessageSize.Value)
                    {
                        int bufferRemainingBytes = receivedBuffer.Length - token.DataStartOffset;
                        int messageRemainingBytes = token.MessageSize.Value - token.ReceivedMessageBytesCount;
                        int bytesToRead = Math.Min(bufferRemainingBytes, messageRemainingBytes);
                        Buffer.BlockCopy(receivedBuffer, token.DataStartOffset, token.MessageData, token.ReceivedMessageBytesCount, bytesToRead);
                        token.ReceivedMessageBytesCount += bytesToRead;
                        token.DataStartOffset += bytesToRead;
                    }

                    if (token.ReceivedMessageBytesCount == token.MessageSize.Value)
                    {
                        byte[] messageData = packetProcessor.IncludeHeader
                            ? token.HeaderData.Concat(token.MessageData).ToArray()
                            : token.MessageData;

                        token.Reset();
                        token.MessageHandler?.Invoke(messageData);
                    }
                }
            }

            token.DataStartOffset = 0;
        }
    }
}

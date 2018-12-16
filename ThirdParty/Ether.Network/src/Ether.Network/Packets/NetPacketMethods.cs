using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ether.Network.Packets
{
    internal static class NetPacketMethods
    {
        /// <summary>
        /// Read methods dictionary.
        /// </summary>
        internal static readonly Dictionary<Type, Func<BinaryReader, object>> ReadMethods = new Dictionary<Type, Func<BinaryReader, object>>
        {
            { typeof(char), reader => reader.ReadChar() },
            { typeof(byte), reader => reader.ReadByte() },
            { typeof(sbyte), reader => reader.ReadSByte() },
            { typeof(bool), reader => reader.ReadBoolean()},
            { typeof(ushort), reader => reader.ReadUInt16()},
            { typeof(short), reader => reader.ReadInt16()},
            { typeof(uint), reader => reader.ReadUInt32()},
            { typeof(int), reader => reader.ReadInt32()},
            { typeof(ulong), reader => reader.ReadUInt64()},
            { typeof(long), reader => reader.ReadInt64()},
            { typeof(float), reader => reader.ReadSingle() },
            { typeof(double), reader => reader.ReadDouble() },
            { typeof(byte[]), reader => reader.ReadBytes(count: reader.ReadInt32()) },
            { typeof(string), reader => new string(reader.ReadChars(count: reader.ReadInt32())) },
        };

        /// <summary>
        /// Write methods dictionary.
        /// </summary>
        internal static readonly Dictionary<Type, Action<BinaryWriter, object>> WriteMethods = new Dictionary<Type, Action<BinaryWriter, object>>
        {
            { typeof(char), (writer, value) => writer.Write((char)value) },
            { typeof(byte), (writer, value) => writer.Write((byte)value) },
            { typeof(bool), (writer, value) => writer.Write((bool)value) },
            { typeof(ushort), (writer, value) => writer.Write((ushort)value) },
            { typeof(short), (writer, value) => writer.Write((short)value) },
            { typeof(uint), (writer, value) => writer.Write((uint)value) },
            { typeof(int), (writer, value) => writer.Write((int)value) },
            { typeof(ulong), (writer, value) => writer.Write((ulong)value) },
            { typeof(long), (writer, value) => writer.Write((long)value) },
            { typeof(float), (writer, value) => writer.Write((float)value) },
            { typeof(double), (writer, value) => writer.Write((double)value) },
            { typeof(byte[]),
                (writer, value) =>
                {
                    writer.Write(((byte[])value).Length);
                    if (((byte[])value).Length > 0)
                        writer.Write((byte[])value);
                }
            },
            { typeof(string),
                (writer, value) =>
                {
                    writer.Write(value.ToString().Length);
                    if (value.ToString().Length > 0)
                        writer.Write(Encoding.ASCII.GetBytes(value.ToString()));
                }
            }
        };
    }
}

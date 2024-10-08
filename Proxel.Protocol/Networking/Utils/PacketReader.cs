﻿using Proxel.Protocol.Helpers;
using Proxel.Protocol.Structs;
using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Networking.Utils
{
    public static class PacketReader
    {
        public static async Task<Packet> ReadPacketAsync(Stream stream, bool readData = true)
        {
            int length = await VarInt.ReadVarIntAsync(stream);
            byte packetId = (byte)await VarInt.ReadVarIntAsync(stream);
            byte[] data;
            if (readData)
            {
                int dataSize = length - VarInt.GetVarIntSize(packetId);
                data = new byte[dataSize];

                int bytesRead = 0;
                while (bytesRead < dataSize)
                {
                    int read = await stream.ReadAsync(data, bytesRead, dataSize - bytesRead);
                    if (read == 0) throw new IOException("Unexpected end of stream.");
                    bytesRead += read;
                }
            }
            else
            {
                data = [];
            }
            return new Packet(packetId, data);
        }
    }
}

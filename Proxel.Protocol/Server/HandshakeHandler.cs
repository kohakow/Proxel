﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Proxel.Log4Console;
using Proxel.Protocol.Helpers;
using Proxel.Protocol.Networking.Utils;
using Proxel.Protocol.Structs;

namespace Proxel.Protocol.Server
{
    public class HandshakeHandler
    {
        internal static async Task HandleHandshakeAsync(Packet packet, NetworkStream networkStream, TcpClient client)
        {
            BinaryReader packetReader = new(new MemoryStream(packet.Data));
            PlayerConnectionInfo playerConnectionInfo = new((ushort)await VarInt.ReadVarIntAsync(packetReader.BaseStream), await FieldReader.ReadStringAsync(packetReader.BaseStream), FieldReader.ReadUnsignedShort(packetReader.BaseStream));
            ushort nextState = (ushort)await VarInt.ReadVarIntAsync(packetReader.BaseStream);
            Log.Debug($"Protocol: {playerConnectionInfo.ProtocolVersion} Type: {nextState} Endpoint: {playerConnectionInfo.ServerAddress}:{playerConnectionInfo.ServerPort}", "HandleHandshakeAsync");

            switch (nextState)
            {
                case 1: // Status
                    await StatusHandler.HandleStatusRequestAsync(networkStream);
                    NetworkStreamDisposer.Dispose(networkStream);
                    break;
                case 2: // Login
                    await LoginHandler.HandleLoginRequestAsync(networkStream, playerConnectionInfo);
                    NetworkStreamDisposer.Dispose(networkStream);
                    break;
                case 3: // Transfer
                    throw new NotImplementedException();
                default:
                    throw new NotSupportedException($"Unsupported Handshake next state: {nextState}");
            }
        }
    }
}

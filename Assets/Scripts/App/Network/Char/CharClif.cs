using System.Collections.Generic;
using Oasis.Network.Common;
using Oasis.Controllers.Char;
using UnityEngine;

namespace Oasis.Network.Char
{
    public static class CharClif
    {
        // Este método é chamado pelo seu NetworkManager principal
        public static void Dispatch(ushort packetId, byte[] data)
        {
            switch (packetId)
            {
                case (ushort)PacketType.CHAR_LIST:
                Handlers.CharListHandler.Handle(data);
                break;
                // Adicione os outros casos aqui (ex: PACKET_CHAR_CREATE_SUCCESS)
            }
        }
    }
}
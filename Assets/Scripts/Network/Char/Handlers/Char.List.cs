using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oasis.Network.Common;
using Oasis.Controllers.Char;

namespace Oasis.Network.Char.Handlers
{
    public static class CharListHandler
    {
        public static void Handle(byte[] data)
        {
            // O primeiro byte do buffer é o 'count' (quantidade de personagens)
            byte charCount = data[0];
            
            int structSize = Marshal.SizeOf(typeof(PACKET_CHAR_LIST_ENTRY));
            List<PACKET_CHAR_LIST_ENTRY> charList = new List<PACKET_CHAR_LIST_ENTRY>();

            // Iteramos sobre a quantidade recebida e extraímos cada struct
            for (int i = 0; i < charCount; i++)
            {
                // Calculamos o deslocamento: pulamos o primeiro byte (count) 
                // e avançamos i * tamanho do struct
                int offset = 1 + (i * structSize);
                
                byte[] structBuffer = new byte[structSize];
                Buffer.BlockCopy(data, offset, structBuffer, 0, structSize);
                
                // Converte os bytes puros de volta para o seu struct PACKET_CHAR_LIST_ENTRY
                charList.Add(BytesToStruct<PACKET_CHAR_LIST_ENTRY>(structBuffer));
            }

            // Envia a lista populada para o Controller que cuida da UI
            if (CharController.Instance != null)
            {
                CharController.Instance.OnReceiveCharList(charList);
            }
        }

        // Método auxiliar para converter o buffer de bytes em um Struct (Memory Layout)
        private static T BytesToStruct<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
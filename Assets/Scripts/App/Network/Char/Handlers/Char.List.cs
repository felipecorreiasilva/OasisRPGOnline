using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oasis.Network.Common;
using Oasis.Controllers.Char;
using UnityEngine;

namespace Oasis.Network.Char.Handlers
{
    public static class CharListHandler
    {
        private static readonly int STRUCT_SIZE = 34; // Tamanho fixo baseado na sua struct

        public static void Handle(byte[] data)
        {
            if (data == null || data.Length < STRUCT_SIZE)
            {
                Debug.LogError("[CharClif] Pacote muito pequeno para conter dados de personagem.");
                return;
            }

            // Se o pacote for um múltiplo exato de 34, não existe cabeçalho de 1 byte.
            // Se o pacote for 69, então o primeiro byte é o count.
            int charCount;
            int offsetStart;

            if (data.Length % STRUCT_SIZE == 0)
            {
                charCount = data.Length / STRUCT_SIZE;
                offsetStart = 0; // Sem cabeçalho
            }
            else
            {
                charCount = data[0];
                offsetStart = 1; // Com cabeçalho
            }
            
            Debug.Log($"[CharClif] Processando {charCount} personagens. Total recebido: {data.Length} bytes.");

            List<PACKET_CHAR_LIST_ENTRY> charList = new List<PACKET_CHAR_LIST_ENTRY>();

            for (int i = 0; i < charCount; i++)
            {
                int offset = offsetStart + (i * STRUCT_SIZE);
                
                // Segurança extra contra estouro de array
                if (offset + STRUCT_SIZE > data.Length) break;

                byte[] structBuffer = new byte[STRUCT_SIZE];
                Buffer.BlockCopy(data, offset, structBuffer, 0, STRUCT_SIZE);
                
                charList.Add(BytesToStruct<PACKET_CHAR_LIST_ENTRY>(structBuffer));
            }

            if (CharController.Instance != null)
            {
                CharController.Instance.OnReceiveCharList(charList);
            }
        }

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
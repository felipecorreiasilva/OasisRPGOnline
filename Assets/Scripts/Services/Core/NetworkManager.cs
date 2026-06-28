using UnityEngine;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;   // Necessário para MemoryStream e BinaryWriter
using Oasis.Network.Common;

namespace Oasis.Network
{
    public class NetworkManager : MonoBehaviour
    {
        // Singleton: permite acesso global como NetworkManager.Instance
        public static NetworkManager Instance { get; private set; }
        public uint CurrentUserId { get; private set; }

        private TcpClient _client;
        private NetworkStream _stream;

        public static event Action<bool, string> OnLoginResult;

        void Awake()
        {
            // Garante que só exista um NetworkManager na cena
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            // AGORA BUSCA A CONFIGURAÇÃO DO DATAMANAGER
            if (DataManager.Instance != null && DataManager.Instance.ServerInfo != null)
            {
                string ip = DataManager.Instance.ServerInfo.Address;
                int port = DataManager.Instance.ServerInfo.Port;
                ConnectToServer(ip, port);
            }
            else
            {
                Debug.LogError("[Network] DataManager não inicializado ou sem dados do servidor!");
            }
        }

        private void ConnectToServer(string ip, int port)
        {
            try
            {
                _client = new TcpClient(ip, port);
                _stream = _client.GetStream();
                Debug.Log($"Conectado ao Emulador Oasis em {ip}:{port}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao conectar: {e.Message}");
            }
        }

        // Método para enviar o login ao servidor
        public void SendLoginRequest(string username, string password)
        {
            if (_stream == null || !_client.Connected)
            {
                Debug.LogError("[Network] Não conectado ao servidor!");
                return;
            }

            PACKET_LOGIN_REQUEST packet = new PACKET_LOGIN_REQUEST();
            packet.packet_id = (ushort)PacketType.LOGIN_REQUEST;
            
            // Atribuição correta aos campos do struct
            packet.username = username;
            packet.password = password;

            byte[] data = StructureToBytes(packet);
            _stream.Write(data, 0, data.Length);
            
            Debug.Log($"[Network] Enviando login: {username}");
        }

        public void SendCharListRequest(uint userId)
        {
            if (_stream == null || !_client.Connected)
            {
                Debug.LogError("[Network] Não conectado ao servidor!");
                return;
            }

            using (MemoryStream m = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write((ushort)0x0200); // ID: CHAR_LIST_REQUEST
                writer.Write(userId);         // UserID
                
                byte[] packet = m.ToArray();
                _stream.Write(packet, 0, packet.Length); // USANDO O _stream AQUI
                
                Debug.Log($"[Network] Enviando solicitação de CharList para user_id: {userId}");
            }
        }

        private byte[] StructureToBytes<T>(T structure)
        {
            int size = Marshal.SizeOf(structure);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        void Update()
        {
            if (_stream != null && _stream.DataAvailable)
            {
                ProcessIncomingPackets();
            }
        }

        private void ProcessIncomingPackets()
        {
            byte[] headerBuffer = new byte[2];
            _stream.Read(headerBuffer, 0, 2);
            ushort packetId = BitConverter.ToUInt16(headerBuffer, 0);
            switch ((PacketType)packetId)
            {
                case PacketType.LOGIN_ACCEPTED:
                    HandleLoginAccepted(headerBuffer);
                    break;
                case PacketType.LOGIN_DENIED:
                    HandleLoginDenied(headerBuffer);
                    break;
                case PacketType.CHAR_SERVER_INFO:
                    HandleCharServerInfo(headerBuffer); 
                    break;
                case PacketType.CHAR_LIST:
                    HandleCharList(headerBuffer);
                    break;
                    default:
                    Debug.LogWarning($"Pacote não mapeado: 0x{packetId:X4}");
                    break;
            }
        }

        private void HandleLoginAccepted(byte[] header)
        {
            PACKET_LOGIN_ACCEPTED data = ReadPacketBody<PACKET_LOGIN_ACCEPTED>(header);
            
            // ATUALIZAÇÃO: Salva o ID recebido do servidor
            CurrentUserId = data.user_id; 
            
            Debug.Log($"[Login] Sucesso! UserID: {data.user_id}");
            OnLoginResult?.Invoke(true, "Login realizado.");
        }

        private void HandleLoginDenied(byte[] header)
        {
            PACKET_LOGIN_DENIED data = ReadPacketBody<PACKET_LOGIN_DENIED>(header);
            Debug.LogWarning($"[Login] Negado! Código: {data.error_code}");
            OnLoginResult?.Invoke(false, "Falha no Login: Código " + data.error_code);
        }

        private void HandleCharServerInfo(byte[] header)
        {
            // Lê o corpo do pacote que contém IP e Porta
            PACKET_CHAR_SERVER_INFO data = ReadPacketBody<PACKET_CHAR_SERVER_INFO>(header);
            
            Debug.Log($"[Login] Redirecionamento recebido! Conectando ao Char Server em {data.port}...");

            // 1. Fecha a conexão atual com o Login Server
            if (_client != null)
            {
                _stream.Close();
                _client.Close();
            }

            // 2. Atualiza a porta do servidor
            ConnectToServer(DataManager.Instance.ServerInfo.Address, (int)data.port);
            SendCharListRequest(CurrentUserId);
        }

        private void HandleCharList(byte[] header)
        {
            // 1. Lê a quantidade de personagens (você já tinha essa lógica)
            byte[] countBuffer = new byte[1];
            _stream.Read(countBuffer, 0, 1);
            byte charCount = countBuffer[0];
            
            Debug.Log($"[Network] Recebendo lista de {charCount} personagens...");

            // 2. Calcula o tamanho e lê os dados
            int structSize = Marshal.SizeOf(typeof(PACKET_CHAR_LIST_ENTRY));
            int totalDataSize = charCount * structSize;
            byte[] dataBuffer = new byte[totalDataSize];
            
            int bytesRead = _stream.Read(dataBuffer, 0, totalDataSize);
            
            // 3. Despacha (agora com o cast correto)
            if (bytesRead == totalDataSize)
            {
                Char.CharClif.Dispatch((ushort)PacketType.CHAR_LIST, dataBuffer);
            }
            else
            {
                Debug.LogError($"[Network] Erro: Lidos {bytesRead} bytes, esperado {totalDataSize}");
            }
        }

        private T ReadPacketBody<T>(byte[] header) where T : struct
        {
            int bodySize = Marshal.SizeOf<T>() - 2;
            byte[] body = new byte[bodySize];
            _stream.Read(body, 0, bodySize);

            byte[] fullPacket = new byte[header.Length + body.Length];
            header.CopyTo(fullPacket, 0);
            body.CopyTo(fullPacket, 2);

            GCHandle handle = GCHandle.Alloc(fullPacket, GCHandleType.Pinned);
            T data = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();
            return data;
        }

        void OnApplicationQuit()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}
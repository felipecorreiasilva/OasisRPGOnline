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
        public uint CurrentUserId { get; private set; } // Adicione esta linha

        
        [Header("Configurações")]
        [SerializeField] private string serverIp = "127.0.0.1";
        [SerializeField] private int serverPort = 6900;

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
            ConnectToServer(serverIp, serverPort);
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
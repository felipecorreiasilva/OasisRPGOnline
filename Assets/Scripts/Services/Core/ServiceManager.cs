using Oasis.Network;
using UnityEngine;

namespace Oasis.Services.Core
{
    public class ServiceManager : MonoBehaviour
    {
        public static ServiceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeServices();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeServices()
        {
            // Ordem de inicialização é importante:
            // 1. DataManager (já deve existir via instância própria ou ser criado aqui)
            // 2. NetworkManager (depende do DataManager)
            // 3. Gerenciadores de UI/Cursor/Sound
            
            gameObject.AddComponent<NetworkManager>();
            Debug.Log("[ServiceManager] NetworkManager criado com sucesso.");
            // gameObject.AddComponent<CursorManager>();
            Debug.Log("[ServiceManager] CursorManager criado com sucesso.");
            // gameObject.AddComponent<SoundManager>(); // Adicione conforme for criando
            
            Debug.Log("[ServiceManager] Todos os serviços foram inicializados.");
        }
    }
}
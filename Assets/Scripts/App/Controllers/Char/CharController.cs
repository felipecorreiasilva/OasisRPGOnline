using UnityEngine;
using System.Collections.Generic;
using Oasis.Network.Common;
using Oasis.UI.Char;

namespace Oasis.Controllers.Char
{
    public class CharController : MonoBehaviour
    {
        public static CharController Instance { get; private set; }

        public CharSelectUI charSelectUI; 
        
        // Cache para armazenar personagens se a UI ainda não estiver pronta
        private List<PACKET_CHAR_LIST_ENTRY> _pendingCharList;

        private void Awake()
        {
            Instance = this;
        }

        public void InitializeCharSelection()
        {
            if (charSelectUI == null)
            {
                charSelectUI = FindAnyObjectByType<CharSelectUI>(FindObjectsInactive.Include);
            }
            
            if (charSelectUI != null)
            {
                Debug.Log("[CharController] Inicializando tela de seleção...");
                
                // 1. Cria os slots e mapeia
                charSelectUI.InitializeSlots();
                charSelectUI.RefreshSlotMap();
                
                // 2. Se tínhamos personagens esperando, processa agora que a UI está pronta
                if (_pendingCharList != null)
                {
                    Debug.Log("[CharController] Processando lista em cache...");
                    charSelectUI.UpdateAllSlots(_pendingCharList);
                    _pendingCharList = null; // Limpa o cache
                }
            }
            else
            {
                Debug.LogError("[CharController] CharSelectUI não encontrado na cena!");
            }
        }

        public void OnReceiveCharList(List<PACKET_CHAR_LIST_ENTRY> characterEntries)
        {
            Debug.Log($"[CharController] Recebidos {characterEntries.Count} personagens.");

            if (charSelectUI == null)
            {
                charSelectUI = FindAnyObjectByType<CharSelectUI>(FindObjectsInactive.Include);
            }

            // Se a UI ainda não tem slots mapeados, armazena no cache
            if (charSelectUI != null && charSelectUI.GetSlotMapCount() > 0)
            {
                charSelectUI.UpdateAllSlots(characterEntries);
            }
            else
            {
                Debug.Log("[CharController] UI não pronta. Armazenando personagens em cache.");
                _pendingCharList = characterEntries;
            }
        }
    }
}
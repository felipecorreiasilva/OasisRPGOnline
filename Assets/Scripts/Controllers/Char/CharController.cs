using UnityEngine;
using System.Collections.Generic;
using Oasis.Network.Common;
using Oasis.UI.Char;

namespace Oasis.Controllers.Char
{
    public class CharController : MonoBehaviour
    {
        public static CharController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void InitializeCharSelection()
        {
            // Busca a UI na cena
            var ui = FindAnyObjectByType<CharSelectUI>();
            
            if (ui != null)
            {
                // Aqui podemos opcionalmente limpar os slots ou definir um estado inicial
                Debug.Log("[CharController] Inicializando tela de seleção...");
                ui.InitializeSlots();
            }
            else
            {
                Debug.LogError("[CharController] CharSelectUI não encontrado na cena!");
            }
        }

        public void OnReceiveCharList(List<PACKET_CHAR_LIST_ENTRY> characterEntries)
        {
            var ui = FindAnyObjectByType<CharSelectUI>();
            if (ui == null) 
            {
                Debug.LogError("[CharController] CharSelectUI não encontrado ao tentar processar lista!");
                return;
            }

            // Agora usamos o método coordenador que criamos na UI
            ui.UpdateAllSlots(characterEntries);
        }
    }
}
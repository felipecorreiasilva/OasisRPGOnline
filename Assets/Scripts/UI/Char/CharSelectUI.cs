using UnityEngine;
using System.Collections.Generic; 
using Oasis.Network.Common;      
using TMPro;                     

namespace Oasis.UI.Char
{
    public class CharSelectUI : MonoBehaviour
    {
        public Transform gridGroup; 
        public GameObject charSlotPrefab; // ARRASTE O PREFAB AQUI NO INSPECTOR

        // 1. Método para garantir que os 9 slots existam na cena
        // Dentro do seu CharSelectUI.cs
        public void InitializeSlots()
        {
            foreach (Transform child in gridGroup) {
                Destroy(child.gameObject);
            }

            for (int i = 1; i <= 9; i++)
            {
                GameObject slotObj = Instantiate(charSlotPrefab, gridGroup);
                
                // 1. Pega o componente CharSlot que está no prefab instanciado
                CharSlot slotScript = slotObj.GetComponent<CharSlot>();
                
                // 2. Chama o Setup passando o índice correto (1 a 9)
                if (slotScript != null)
                {
                    slotScript.Setup(i);
                }
                
                slotObj.name = $"Char_Slot_{i}";
            }
        }

        // 2. Método coordenador para atualizar os dados dos slots existentes
        public void UpdateAllSlots(List<PACKET_CHAR_LIST_ENTRY> characters)
        {
            // 1. Criar um mapa para busca rápida (Onde o char_num é a chave)
            Dictionary<byte, PACKET_CHAR_LIST_ENTRY> charMap = new Dictionary<byte, PACKET_CHAR_LIST_ENTRY>();
            foreach (var entry in characters)
            {
                charMap[entry.char_num] = entry;
            }

            // 2. Percorrer de 1 a 9 (Total de slots padrão)
            for (int i = 1; i <= 9; i++)
            {
                bool exists = charMap.ContainsKey((byte)i);
                
                // Se existir, passamos os dados do personagem, se não, passamos default
                PACKET_CHAR_LIST_ENTRY data = exists ? charMap[(byte)i] : default;
                
                // Atualiza a UI para o slot i
                UpdateSlot(i, exists, data);
            }
        }

        public void UpdateSlot(int index, bool exists, PACKET_CHAR_LIST_ENTRY data)
        {
            // Busca o slot pelo nome
            Transform slot = gridGroup.Find($"Char_Slot_{index}");
            if (slot == null) 
            {
                Debug.LogWarning($"[CharSelectUI] Slot Char_Slot_{index} não encontrado.");
                return;
            }

            // Gerenciamento de visibilidade
            var newButton = slot.Find("New");
            var preview = slot.Find("Char_Preview");
            
            if (newButton != null) newButton.gameObject.SetActive(!exists);
            if (preview != null) preview.gameObject.SetActive(exists);

            if (exists)
            {
                // Usa a propriedade auxiliar que criamos no struct
                var nameText = slot.Find("Name")?.GetComponent<TMPro.TextMeshProUGUI>();
                if (nameText != null) 
                {
                    nameText.text = data.CharName;
                    Debug.Log($"[CharSelectUI] Slot {index} atualizado com nome: {data.CharName}");
                }
            }
        }
    }
}
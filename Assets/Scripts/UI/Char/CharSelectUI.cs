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
            var charMap = new Dictionary<byte, PACKET_CHAR_LIST_ENTRY>();
            foreach (var entry in characters) charMap[entry.char_num] = entry;

            for (int i = 1; i <= 9; i++)
            {
                bool exists = charMap.ContainsKey((byte)i);
                PACKET_CHAR_LIST_ENTRY data = exists ? charMap[(byte)i] : default;
                
                UpdateSlot(i, exists, data);
            }
        }

        public void UpdateSlot(int index, bool exists, PACKET_CHAR_LIST_ENTRY data)
        {
            Transform slot = gridGroup.Find($"Char_Slot_{index}");
            if (slot == null) return;

            // Ativa o botão "New" se não existir personagem
            var newBtn = slot.Find("New");
            var preview = slot.Find("Char_Preview");

            if (newBtn != null) newBtn.gameObject.SetActive(!exists);
            if (preview != null) preview.gameObject.SetActive(exists);

            if (exists)
            {
                var nameText = slot.Find("Name")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null) nameText.text = data.name;
            }
        }
    }
}
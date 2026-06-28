using UnityEngine;
using System.Collections.Generic; 
using Oasis.Network.Common; 
using TMPro; 

namespace Oasis.UI.Char
{
    public class CharSelectUI : MonoBehaviour
    {
        public Transform gridGroup; 
        public GameObject charSlotPrefab; 
        private Dictionary<int, Transform> slotMap = new Dictionary<int, Transform>();

        // 1. Inicializa os 9 slots
        public void InitializeSlots()
        {
            foreach (Transform child in gridGroup) Destroy(child.gameObject);
            slotMap.Clear();

            for (int i = 1; i <= 9; i++)
            {
                GameObject slotObj = Instantiate(charSlotPrefab, gridGroup);
                slotObj.name = $"Char_Slot_{i}";
                // Mapeamento imediato após a criação
                slotMap[i] = slotObj.transform;
            }
        }

        public void RefreshSlotMap()
        {
            slotMap.Clear();
            int slotIndex = 1;
            foreach (Transform child in gridGroup)
            {
                slotMap[slotIndex] = child;
                slotIndex++;
            }
            Debug.Log($"[CharSelectUI] Mapeamento concluído. {slotMap.Count} slots encontrados.");
        }

        public int GetSlotMapCount()
        {
            return slotMap.Count;
        }

        // 2. Método principal de atualização
        public void UpdateAllSlots(List<PACKET_CHAR_LIST_ENTRY> characterEntries)
        {
            if (slotMap.Count == 0) RefreshSlotMap();

            foreach (var entry in characterEntries)
            {
                // Usamos o char_num como chave (1, 2, 3...)
                if (slotMap.TryGetValue((int)entry.char_num, out Transform slot))
                {
                    // Gerencia visibilidade dos botões
                    slot.Find("New")?.gameObject.SetActive(false);
                    
                    Transform preview = slot.Find("Char_Preview");
                    if (preview != null) preview.gameObject.SetActive(true);

                    // ATUALIZAÇÃO DO NOME (PADRONIZADO)
                    Transform nameTransform = slot.Find("Char_Name");
                    if (nameTransform != null)
                    {
                        nameTransform.gameObject.SetActive(true);
                        var nameText = nameTransform.GetComponent<TextMeshProUGUI>();
                        if (nameText != null) 
                        {
                            nameText.text = entry.CharName;
                            Debug.Log($"[CharSelectUI] Slot {(int)entry.char_num} atualizado: {entry.CharName}");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"[CharSelectUI] Slot não encontrado para char_num: {entry.char_num}");
                }
            }
        }
    }
}
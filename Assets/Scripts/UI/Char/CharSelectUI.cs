using UnityEngine;

namespace Oasis.UI.Char
{
    public class CharSelectUI : MonoBehaviour
    {
        [SerializeField] private GameObject charSlotPrefab;
        [SerializeField] private Transform gridGroup;
        [SerializeField] private int numberOfSlots = 9;

        public void InitializeSlots()
        {
            // Limpa qualquer coisa que já esteja lá
            foreach (Transform child in gridGroup)
            {
                Destroy(child.gameObject);
            }

            // Instancia os novos slots
            for (int i = 0; i < numberOfSlots; i++)
            {
                GameObject slot = Instantiate(charSlotPrefab, gridGroup);
        
                slot.name = $"Char_Slot_{i + 1}";
            }
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems; // Necessário para os eventos de mouse

namespace Oasis.UI.Char
{
    public class CharSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        // Esta variável guarda o índice ou ID do personagem deste slot específico
        private int slotIndex;

        public void Setup(int index)
        {
            slotIndex = index;
            // Aqui você pode mudar o nome do objeto para facilitar o debug
            gameObject.name = $"Char_Slot_{index}";
        }

        // Hover: Ocorre quando o mouse entra no slot
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"[CharSlot] Hover no slot: {slotIndex}");
            // Exemplo: mudar a cor aqui ou tocar um som de "seleção"
        }

        // Clique: Detecta cliques (incluindo o duplo)
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                Debug.Log($"[CharSlot] CLIQUE DUPLO no slot: {slotIndex}. Entrando no jogo...");
                // Aqui você chama o CharController para confirmar a entrada
            }
        }
    }
}
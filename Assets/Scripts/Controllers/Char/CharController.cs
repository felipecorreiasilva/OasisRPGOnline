using UnityEngine; // Necessário para MonoBehaviour e FindObjectOfType
using Oasis.UI.Char; // Necessário para acessar CharSelectUI

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
            // Busca diretamente na cena
            var ui = FindAnyObjectByType<CharSelectUI>();
            
            if (ui != null)
            {
                ui.InitializeSlots();
            }
            else
            {
                Debug.LogError("[CharController] CharSelectUI não encontrado na cena!");
            }
        }
    }
}
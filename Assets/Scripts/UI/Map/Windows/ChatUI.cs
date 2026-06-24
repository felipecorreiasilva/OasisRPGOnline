using UnityEngine;
using TMPro;
using Oasis.Network; // Namespace do seu NetworkManager

namespace Oasis.UI.Windows
{
    public class ChatUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _chatInput;
        [SerializeField] private TextMeshProUGUI _chatLog;

        void Update()
        {
            // Atalho para focar no chat (Enter)
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_chatInput.isFocused)
                    SendMessageToNetwork();
                else
                    _chatInput.ActivateInputField();
            }
        }

        private void SendMessageToNetwork()
        {
            if (string.IsNullOrWhiteSpace(_chatInput.text)) return;

            string message = _chatInput.text;
            Debug.Log($"Enviando: {message}");
            
            // Aqui você chamará o método do seu NetworkManager
            // NetworkManager.Instance.SendChatPacket(message);

            _chatInput.text = ""; // Limpa o campo
            _chatInput.DeactivateInputField();
        }

        // Chamado pelo NetworkManager quando um pacote de chat é recebido
        public void AppendMessage(string sender, string message)
        {
            _chatLog.text += $"\n[{sender}]: {message}";
        }
    }
}
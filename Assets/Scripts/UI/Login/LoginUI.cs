using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Oasis.Network; // Namespace do seu NetworkManager

namespace Oasis.UI.Login
{
    public class LoginUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Button _loginButton;

        private void OnEnable()
        {
            // Escuta a resposta do servidor
            NetworkManager.OnLoginResult += HandleLoginResult;
        }

        private void OnDisable()
        {
            // Remove o listener ao destruir ou desativar a tela
            NetworkManager.OnLoginResult -= HandleLoginResult;
        }

        private void Start()
        {
            _loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        public void OnLoginButtonClicked()
        {
            string user = _usernameInput.text;
            string pass = _passwordInput.text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                Debug.LogWarning("Por favor, preencha usuário e senha.");
                return;
            }

            // Chama o método no NetworkManager (Singleton)
            NetworkManager.Instance.SendLoginRequest(user, pass);
        }

        private void HandleLoginResult(bool success, string message)
        {
            if (success)
            {
                Debug.Log("Login Aceito: " + message);
                // TODO: Adicionar aqui a troca de cena para Seleção de Personagem
            }
            else
            {
                Debug.LogError("Erro no Login: " + message);
                // TODO: Adicionar aviso visual na tela (MessagePopupUI)
            }
        }
    }
}
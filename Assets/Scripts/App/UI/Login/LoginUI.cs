using UnityEngine;
using TMPro; // Certifique-se de usar o namespace do TextMeshPro
using Oasis.Network; // Para acessar o NetworkManager

namespace Oasis.UI.Login
{
    public class LoginUI : MonoBehaviour
    {
        [Header("Referências de Input")]
        [SerializeField] private TMP_InputField _usernameInput;
        [SerializeField] private TMP_InputField _passwordInput;

        /// <summary>
        /// Este método deve estar vinculado ao evento "OnClick" do seu botão Entrar no Inspector.
        /// </summary>
        public void OnLoginButtonClicked()
        {
            string user = _usernameInput.text;
            string pass = _passwordInput.text;

            // Validação simples local antes de enviar
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                Debug.LogWarning("[LoginUI] Usuário ou senha vazios!");
                // Aqui você poderia chamar um MessagePopupUI para avisar o jogador
                return;
            }

            // 1. Esconde a tela de login
            UIManager.Instance.loginPanel.SetActive(false);
            
            // 2. Mostra a tela de "Aguarde"
            UIManager.Instance.ShowAwaitScreen(true);

            // 3. Solicita ao NetworkManager o envio (o Controller ouvirá a resposta)
            NetworkManager.Instance.SendLoginRequest(user, pass);
        }
    }
}
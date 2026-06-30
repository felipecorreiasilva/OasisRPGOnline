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
    // Verifica se os Managers existem antes de acessar
    if (UIManager.Instance == null) {
        Debug.LogError("[LoginUI] UIManager não encontrado!");
        return;
    }
    if (NetworkManager.Instance == null) {
        Debug.LogError("[LoginUI] NetworkManager não encontrado! Verifique o ServiceManager.");
        return;
    }

    string user = _usernameInput.text;
    string pass = _passwordInput.text;

    if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) return;

    UIManager.Instance.loginPanel.SetActive(false);
    UIManager.Instance.ShowAwaitScreen(true);
    
    // Agora é seguro chamar, pois você validou acima
    NetworkManager.Instance.SendLoginRequest(user, pass);
}
    }
}
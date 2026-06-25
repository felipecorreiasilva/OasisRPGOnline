using UnityEngine;
using System.Collections; // Necessário para Coroutines
using Oasis.Network;
using Oasis.UI;

namespace Oasis.Controllers.Login
{
    public class LoginController : MonoBehaviour
    {
        private void OnEnable()
        {
            NetworkManager.OnLoginResult += HandleLoginResult;
        }

        private void OnDisable()
        {
            NetworkManager.OnLoginResult -= HandleLoginResult;
        }

        private void HandleLoginResult(bool success, string message)
        {
            if (success)
            {
                Debug.Log($"[LoginController] Sucesso! Iniciando transição...");
                
                // Dispara a Coroutine em vez de chamar direto
                StartCoroutine(TransitionToCharSelect());
            }
            else
            {
                // Erro: Fecha o "Aguarde" instantaneamente
                UIManager.Instance.ShowAwaitScreen(false);
                UIManager.Instance.loginPanel.SetActive(true);
            }
        }

        // Coroutine: Aguarda 1 segundo e depois executa a troca
        private IEnumerator TransitionToCharSelect()
        {
            // 1. Solicita a lista de personagens
            uint userId = NetworkManager.Instance.CurrentUserId;
            NetworkManager.Instance.SendCharListRequest(userId);

            // 2. Espera 1 segundo (tempo em tempo real, independente de timescale)
            yield return new WaitForSeconds(1.0f);

            // 3. Executa a troca visual
            UIManager.Instance.ShowCharSelectScreen();
            
            Debug.Log("[LoginController] Transição concluída.");
        }
    }
}
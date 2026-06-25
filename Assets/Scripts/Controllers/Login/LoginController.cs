using UnityEngine;
using System.Collections;
using Oasis.Network;
using Oasis.Controllers.Char;

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
                StartCoroutine(TransitionToCharSelect());
            }
            else
            {
                UIManager.Instance.ShowAwaitScreen(false);
                UIManager.Instance.loginPanel.SetActive(true);
            }
        }

        private IEnumerator TransitionToCharSelect()
        {
            // 1. Solicita a lista de personagens ao servidor
            uint userId = NetworkManager.Instance.CurrentUserId;
            NetworkManager.Instance.SendCharListRequest(userId);

            // 2. Aguarda um curto período para a transição
            yield return new WaitForSeconds(0.7f);

            // 3. Executa a troca visual
            UIManager.Instance.ShowCharSelectScreen();

            // 4. Delega a inicialização da lista ao CharController
            // Isso mantém o LoginController focado apenas no fluxo de login
            if (CharController.Instance != null)
            {
                CharController.Instance.InitializeCharSelection();
            }
            else
            {
                Debug.LogError("[LoginController] CharController não encontrado na cena!");
            }
            
            Debug.Log("[LoginController] Transição e chamada ao CharController concluídas.");
        }
    }
}
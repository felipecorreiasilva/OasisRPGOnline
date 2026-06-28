using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Janelas da UI")]
    public GameObject loginPanel;
    public GameObject pleaseAwaitPanel; // Arraste o objeto da tela de "Aguarde..." aqui
    public GameObject charSelectPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // Garante que apenas a tela de login esteja ativa ao iniciar
        loginPanel.SetActive(true);
        pleaseAwaitPanel.SetActive(false);
        charSelectPanel.SetActive(false);
    }

    public void ShowAwaitScreen(bool show)
    {
        pleaseAwaitPanel.SetActive(show);
    }

    public void ShowCharSelectScreen()
    {
        loginPanel.SetActive(false);
        pleaseAwaitPanel.SetActive(false); // Esconde o "Aguarde"
        charSelectPanel.SetActive(true);
        Debug.Log("ShowCharSelectScreen: ");
    }
}
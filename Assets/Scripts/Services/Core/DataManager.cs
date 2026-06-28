using UnityEngine;
using System.Xml.Linq;
using System.IO;

public class DataManager : MonoBehaviour
{
    // Padrão Singleton para acesso fácil de qualquer lugar
    public static DataManager Instance { get; private set; }

    public ServerConfig ServerInfo { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadClientInfo();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadClientInfo()
    {
        // Caminho do arquivo na pasta Assets/data/ (ou dentro de StreamingAssets se preferir)
        string path = Path.Combine(Application.dataPath, "data", "clientinfo.xml");

        if (File.Exists(path))
        {
            XDocument doc = XDocument.Load(path);
            var connection = doc.Element("clientinfo").Element("connection");

            ServerInfo = new ServerConfig
            {
                Display = connection.Element("display").Value,
                Address = connection.Element("address").Value,
                Port = int.Parse(connection.Element("port").Value),
                Version = int.Parse(connection.Element("version").Value)
            };

            Debug.Log($"DataManager: Conectando ao servidor {ServerInfo.Display} em {ServerInfo.Address}:{ServerInfo.Port}");
        }
        else
        {
            Debug.LogError($"DataManager: Arquivo clientinfo.xml não encontrado em {path}");
        }
    }
}

// Classe simples para segurar os dados (Data Transfer Object)
public class ServerConfig
{
    public string Display;
    public string Address;
    public int Port;
    public int Version;
}
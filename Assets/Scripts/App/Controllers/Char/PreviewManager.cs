using UnityEngine;
using System.Collections.Generic;
using System.IO; // Necessário para manipulação de arquivos

public class PreviewManager : MonoBehaviour
{
    // Agora você pode arrastar o arquivo .spr e .act direto para o Inspector
    public Object spriteFile;
    public Object actionFile;
    
    private SpriteParser _spriteParser = new SpriteParser();
    private ActionParser _actionParser = new ActionParser();
    
    private List<Texture2D> _cachedTextures = new List<Texture2D>();
    public List<SpriteRenderer> layerRenderers;

    public float frameRate = 0.1f;
    private float _timer;
    private int _currentActionIndex = 0;
    private int _currentFrameIndex = 0;

    // Método para inicializar automaticamente ao clicar em um botão ou start
    public void LoadFromInspector()
    {
        if (spriteFile == null || actionFile == null)
        {
            Debug.LogError("Por favor, arraste o arquivo .spr e .act para o Inspector!");
            return;
        }

        // Pega o caminho físico do projeto no computador
#if UNITY_EDITOR
        string sprPath = UnityEditor.AssetDatabase.GetAssetPath(spriteFile);
        string actPath = UnityEditor.AssetDatabase.GetAssetPath(actionFile);
        
        string fullSprPath = Path.Combine(Directory.GetCurrentDirectory(), sprPath);
        string fullActPath = Path.Combine(Directory.GetCurrentDirectory(), actPath);

        _spriteParser.LoadSpriteData(fullSprPath);
        _actionParser.LoadActionData(fullActPath);
        
        PrepareCache();
        Debug.Log("Arquivos carregados via Inspector com sucesso!");
#endif
    }

    private void PrepareCache()
    {
        _cachedTextures.Clear();
        for (int i = 0; i < _spriteParser.FramesData.Count; i++)
        {
            _cachedTextures.Add(_spriteParser.GetFrameTexture(i));
        }
    }

    void Update()
    {
        if (_actionParser.Actions.Count == 0 || _cachedTextures.Count == 0) return;

        _timer += Time.deltaTime;
        if (_timer >= frameRate)
        {
            _timer = 0;
            var currentAction = _actionParser.Actions[_currentActionIndex];
            _currentFrameIndex = (_currentFrameIndex + 1) % currentAction.Frames.Count;
            RenderFrame();
        }
    }

    private void RenderFrame()
    {
        var currentFrame = _actionParser.Actions[_currentActionIndex].Frames[_currentFrameIndex];

        for (int i = 0; i < currentFrame.Layers.Count; i++)
        {
            if (i >= layerRenderers.Count) break;

            var layerData = currentFrame.Layers[i];
            
            if (layerData.SpriteIndex < _cachedTextures.Count)
            {
                Texture2D tex = _cachedTextures[layerData.SpriteIndex];
                layerRenderers[i].sprite = Sprite.Create(tex, 
                    new Rect(0, 0, tex.width, tex.height), 
                    new Vector2(0.5f, 0.5f));
                
                layerRenderers[i].transform.localPosition = new Vector3(layerData.X, layerData.Y, 0);
            }
        }
    }
}
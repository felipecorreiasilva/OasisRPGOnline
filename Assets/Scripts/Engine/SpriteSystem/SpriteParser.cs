using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SpriteParser
{
    public Color32[] Palette = new Color32[256];
    public List<byte[]> FramesData = new List<byte[]>();
    public List<Vector2Int> FrameSizes = new List<Vector2Int>();

    public void LoadSpriteData(string filePath)
    {
        if (!File.Exists(filePath)) return;

        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            // 1. Header
            string signature = new string(reader.ReadChars(2));
            short version = reader.ReadInt16();
            short indexedCount = reader.ReadInt16();
            short rgbaCount = reader.ReadInt16(); // Algumas versões usam isso

            // 2. Load Palette (Sempre no final do arquivo .spr)
            reader.BaseStream.Seek(-1024, SeekOrigin.End);
            for (int i = 0; i < 256; i++)
            {
                byte b = reader.ReadByte();
                byte g = reader.ReadByte();
                byte r = reader.ReadByte();
                byte a = reader.ReadByte();
                Palette[i] = new Color32(r, g, b, a);
            }

            // 3. Load Frames (Volta para o início, após o header)
            reader.BaseStream.Seek(6, SeekOrigin.Begin);
            for (int i = 0; i < indexedCount; i++)
            {
                short width = reader.ReadInt16();
                short height = reader.ReadInt16();
                byte[] pixels = reader.ReadBytes(width * height);
                
                FrameSizes.Add(new Vector2Int(width, height));
                FramesData.Add(pixels);
            }
        }
    }

    // Helper para gerar a Texture2D da Unity a partir de um frame
    public Texture2D GetFrameTexture(int frameIndex)
    {
        if (frameIndex < 0 || frameIndex >= FramesData.Count) return null;

        Vector2Int size = FrameSizes[frameIndex];
        byte[] pixels = FramesData[frameIndex];
        Texture2D tex = new Texture2D(size.x, size.y);

        Color32[] colors = new Color32[size.x * size.y];
        for (int i = 0; i < pixels.Length; i++)
        {
            colors[i] = Palette[pixels[i]];
        }

        tex.SetPixels32(colors);
        tex.Apply();
        return tex;
    }
}
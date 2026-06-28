using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAction
{
    public List<AnimationFrame> Frames = new List<AnimationFrame>();
}

public class AnimationFrame
{
    public List<AnimationFrameLayer> Layers = new List<AnimationFrameLayer>();
}

public class AnimationFrameLayer
{
    public int SpriteIndex;
    public int X, Y;
    public bool IsMirrored;
}

public class ActionParser
{
    public List<AnimationAction> Actions = new List<AnimationAction>();

    public void LoadActionData(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            // 1. Header parsing
            string signature = new string(reader.ReadChars(2));
            short version = reader.ReadInt16();
            short actionCount = reader.ReadInt16();
            
            // Skip reserved bytes
            reader.ReadBytes(24); 

            for (int i = 0; i < actionCount; i++)
            {
                AnimationAction action = new AnimationAction();
                short frameCount = reader.ReadInt16();
                
                for (int j = 0; j < frameCount; j++)
                {
                    AnimationFrame frame = new AnimationFrame();
                    // Skip frame delay/events
                    reader.ReadBytes(4); 
                    
                    short layerCount = reader.ReadInt16();
                    for (int k = 0; k < layerCount; k++)
                    {
                        AnimationFrameLayer layer = new AnimationFrameLayer();
                        layer.X = reader.ReadInt32();
                        layer.Y = reader.ReadInt32();
                        layer.SpriteIndex = reader.ReadInt32();
                        
                        // Skip additional layer data (color, angle, etc.)
                        reader.ReadBytes(12); 
                        
                        frame.Layers.Add(layer);
                    }
                    action.Frames.Add(frame);
                }
                Actions.Add(action);
            }
        }
        Debug.Log($"Parsing complete: {Actions.Count} actions loaded.");
    }
}
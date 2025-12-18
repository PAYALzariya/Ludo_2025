using System;
using UnityEngine;
using System.Collections.Generic;
[System. Serializable]
public struct Frame 
{
    public int frameIndex;
    public Sprite frameSprite;
}
[CreateAssetMenu(fileName = "FrameData")]
public class FrameData : ScriptableObject
{

    public Frame[] frames;
    public Dictionary<int, Frame> LookupDictionary = new Dictionary<int, Frame>();
    void OnEnable()
    {
        LookupDictionary = new Dictionary<int, Frame>();
        foreach (var Frame in frames)
        {
            int key = Frame.frameIndex;
            if (!LookupDictionary.ContainsKey(key))
            {
                LookupDictionary.Add(key, Frame);
            }
        }
    }
}

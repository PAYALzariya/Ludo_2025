using System;
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct level
{
    public int LevelNo;
    public string Levelscore;
    public Sprite levelSprite;
}
[CreateAssetMenu(fileName = "Leveldata")]
public class Leveldata : ScriptableObject
{


    public level[] levels;
    public Dictionary<int, level> LookupDictionary = new Dictionary<int, level>();
    void OnEnable()
    {
        LookupDictionary = new Dictionary<int, level>();
        foreach (var level in levels)
        {
            int key = level.LevelNo;
            if (!LookupDictionary.ContainsKey(key))
            {
                LookupDictionary.Add(key, level);
            }
        }
    }
}



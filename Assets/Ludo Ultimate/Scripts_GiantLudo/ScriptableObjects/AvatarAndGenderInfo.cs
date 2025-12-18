using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AvatarAndGenderInfo : ScriptableObject
{
    public AvatarInfo[] avatarInfos;
}

[System.Serializable]
public struct AvatarInfo
{
    public Sprite avatar;
    public string name;
    public Gender gender;
}

public enum Gender
{
    Male,
    Female,
}
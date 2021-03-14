using System;
using UnityEngine;

public enum TeamFlags : int
{
    None = 0,
    GoodGuys = 1 << 0,
    BadDudes = 1 << 1,
    Hostile = 1 << 2
}

[CreateAssetMenu]
public class Team : ScriptableObject
{
    [EnumFlag]
    public TeamFlags flags;
    public Color[] color;
    
}


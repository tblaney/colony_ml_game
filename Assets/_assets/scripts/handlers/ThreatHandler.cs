using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreatHandler : MonoBehaviour, IHandler
{
    public static ThreatHandler Instance;
    public void Initialize()
    {
        Instance = this;
    }
}
[Serializable]
public abstract class Threat
{
    public enum Type
    {
        Enemy,
        Environmental,
    }
    public Type _type;
    public int _prefab;
}
[Serializable]
public class ThreatEnemy : Threat
{
    // 
}
[Serializable]
public class ThreatEnvironment : Threat
{
    // fires, lightning, solar flares
}




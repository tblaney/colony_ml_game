using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreatHandler : MonoBehaviour, IHandler
{
    public static ThreatHandler Instance;

    public List<Threat> _threats;


    public void Initialize()
    {
        Instance = this;
    }
    public void SpawnThreat(int index)
    {
        Threat threat = GetThreat(index);
        // TODO: implement threat spawning system
    }
    public Threat GetThreat(int index)
    {
        foreach (Threat threat in _threats)
        {
            if (threat._index == index)
                return threat;
        }
        return null;
    }
}
[Serializable]
public abstract class Threat : MonoBehaviour
{
    public string _name;
    public int _index;
    public int _cost;
    public int _prefab;

    public ThreatController _controller;

    public virtual void Spawn(Vector3 position)
    {

    }
}
[Serializable]
public class ThreatEnemy : Threat
{
    // singular enemies, groups of enemies
    // TODO: immplement necessary enemy info here (damage, speed)
}
[Serializable]
public class ThreatEnvironment : Threat
{
    // fires, lightning, solar flares
    // I can implement this part - Traven
}




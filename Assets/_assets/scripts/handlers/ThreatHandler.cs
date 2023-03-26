using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreatHandler : MonoBehaviour, IHandler
{
    public static ThreatHandler Instance;
    [SerializeField] List<ThreatEnemy> _enemyThreats;
    [SerializeField] List<ThreatEnvironment> _environmentThreats;
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
public abstract class Threat
{
    public enum Type
    {
        Enemy,
        Environmental,
    }
    public string _name;
    public int _index;

    public Type _type;
    public int _prefab;
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




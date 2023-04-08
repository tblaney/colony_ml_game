using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrefabHandler : MonoBehaviour, IHandler
{
    public static PrefabHandler Instance;
    public List<PrefabInput> _prefabs;
    public void Initialize()
    {
        Instance = this;
    }
    public GameObject GetPrefab(int index)
    {
        foreach (PrefabInput prefab in _prefabs)
        {
            if (prefab._index == index)
                return prefab._prefab;
        }
        return null;
    }
    public GameObject GetPrefab(string name)
    {
        foreach (PrefabInput prefab in _prefabs)
        {
            if (prefab._name == name)
                return prefab._prefab;
        }
        return null;
    }
    public PrefabInput GetPrefabInput(int index)
    {
        foreach (PrefabInput prefab in _prefabs)
        {
            if (prefab._index == index)
                return prefab;
        }
        return default(PrefabInput);
    }
}

[Serializable]
public struct PrefabInput
{
    public string _name;
    public int _index;
    public GameObject _prefab;
}
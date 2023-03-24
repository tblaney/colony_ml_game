using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Node 
{
    public int _prefab;
    public Vector3Int _position;
    public bool _active = true;
    public float _timer;
    public float _refreshRate;
    public int _inventoryIndex = 0;

    public enum Type
    {
        Default,
        Food,
        Mineral,
        Tree,
        BuildingUnfinished,
        Building,
    }
    public Type _type;

    public Node(int prefab)
    {
        _prefab = prefab;
        _active = true;
    }
}

[Serializable]
public class NodeInput
{
    public int _group;
    public int _amount;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingHandler : MonoBehaviour, IHandler
{
    public static BuildingHandler Instance;
    public List<Building> _buildings;

    public void Initialize()
    {
        Instance = this;
    }
    public List<Building> GetBuildings()
    {
        return _buildings;
    }
    public Building GetBuilding(int index)
    {   
        foreach (Building building in _buildings)
        {
            if (building._index == index)
                return building;
        }
        return null;
    }
    public Building GetBuilding(string index)
    {   
        foreach (Building building in _buildings)
        {
            if (building._name == index)
                return building;
        }
        return null;
    }
}
[Serializable]
public class Building
{
    [Header("Define Building:")]
    public string _name;
    public string _description;
    public int _index;
    public int _prefabBuilding;
    public int _prefabBuilt;
    public float _buildTime;
    public List<ItemInput> _itemRequirements;
    public List<ItemInput> _itemOutputs;
    public Node GetNodeBuilding(Vector3Int position)
    {   
        Node node = new Node(_prefabBuilding, position, Node.Type.BuildingUnfinished);
        return node;
    }
    public Node GetNodeBuilt(Vector3Int position)
    {   
        Node node = new Node(_prefabBuilt, position, Node.Type.Building);
        return node;
    }
}
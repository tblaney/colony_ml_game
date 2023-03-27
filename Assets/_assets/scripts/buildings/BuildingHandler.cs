using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingHandler : MonoBehaviour, IHandler
{
    public static BuildingHandler Instance;
    public List<Building> _buildings;
    List<BuiltNodeObject> _activeObjects;

    public void Initialize()
    {
        Instance = this;
        _activeObjects = new List<BuiltNodeObject>();
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
    public BuiltNodeObject GetClosestBuildingObject(int index, Vector3 position)
    {
        float distanceMin = 1000f;
        BuiltNodeObject obj = null;
        foreach (BuiltNodeObject built in _activeObjects)
        {
            Building building = built.GetBuilding();
            if (building._index == index)
            {
                float distance = Vector3.Distance(position, built.transform.position);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    obj = built;
                }
            }
        }
        return obj;
    }
    public BuiltNodeObject GetClosestBuildingObject(string name, Vector3 position)
    {
        Building bldg = GetBuilding(name);
        return GetClosestBuildingObject(bldg._index, position);
    }
    public void RegisterBuiltObject(BuiltNodeObject obj)
    {
        _activeObjects.Add(obj);
    }
    public void UnregisterBuiltObject(BuiltNodeObject obj)
    {
        if (_activeObjects.Contains(obj))
            _activeObjects.Remove(obj);
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
        Node node = new Node(_prefabBuilding){_position = position};
        return node;
    }
    public Node GetNodeBuilt(Vector3Int position)
    {   
        Node node = new Node(_prefabBuilt){_position = position};
        return node;
    }
}
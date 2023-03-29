using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Node : Queueable, ITarget
{
    public string _name;
    public int _prefab;
    public int _inventoryIndex = 0;
    public Vector3Int _position;
    public bool _active = true;
    public bool _surface;
    public bool _busy;
    public float _timer;
    public float _refreshRate;
    public float _propogationFactor = 0f; 
    public Vitality _health;
    public event EventHandler OnPositionChange;
    public enum Type
    {
        Default,
        Food,
        Mineral,
        Tree,
        BuildingUnfinished,
        Building,
    }
    public Type _nodeType;

    public Node(int prefab, Vector3Int position, Type type)
    {
        _prefab = prefab;
        _position = position;
        _nodeType = type;
        _active = true;
        _health = new Vitality() {_name = "health", _val = 100};
    }
    public Node()
    {
        
    }
    public void SetPosition(Vector3Int position)
    {
        _position = position;
        OnPositionChange?.Invoke(null, EventArgs.Empty);
    }
    public void SurfaceCheck()
    {
        if (Mathf.Abs(_position.y - NodeProcessor._boundsHeight) > 0.5f)
        {
            _surface = false;
            return;
        }
        List<Node> neighbours = NodeProcessor._nodeActions.GetNeighboursFunc(this, true);
        if (neighbours.Count < 4)
        {
            _surface = true;
        } else
        {
            _surface = false;
        }
    }
    public void TryPropagate()
    {
        
    }
    public void SetBusy(bool busy)
    {
        _busy = busy;
    }
    public void DestroyNeighbourChecks()
    {
        // check for height drop:
        List<Node> nodes = new List<Node>();
        for (int i = 1; i < 100; i++)
        {
            Vector3Int position = _position + new Vector3Int(0, i, 0);
            Debug.Log("Node Destroy Neighbour Checks: " + position);

            Node node = NodeProcessor._nodeActions.GetNodeFunc(position);
            if (node != null)
            {
                nodes.Add(node);
            } else
            {
                break;
            }
        }
        Debug.Log("Node Destroy Neighbour Checks: " + nodes.Count);
        foreach (Node node in nodes)
        {
            node.SetPosition(node._position - new Vector3Int(0, 1, 0));
        }
        // check for surface
        List<Node> neighbours = NodeProcessor._nodeActions.GetNeighboursFunc(this, true);
        foreach (Node node in neighbours)
        {
            node.SurfaceCheck();
        }
    }
    public Vector3 GetPosition()
    {
        return _position;
    }
    public bool Damage(int val)
    {
        return true;
    }
    public static string GetDescription(Node node)
    {
        switch (node._nodeType)
        {
            default:
            case Type.Default:
                return "A standard earth block.";
                break;
            case Type.Building:
                return "A structural element built by robots.";
                break;
            case Type.BuildingUnfinished:
                return "A blueprint of a to-be-built structural element.";
                break;
            case Type.Mineral:
                return "A collectible raw mineral. To be used in crafting.";
                break;
            case Type.Food:
                return "A collectible edible plant that can sustain your Hab-Bot's organic decomposer.";
                break;
            case Type.Tree:
                return "A collectible tree which provides wood. To be used in crafting.";
                break;
        }
        return "";
    }
    public HabBot.State GetState()
    {
        switch (_nodeType)
        {
            case Type.Building:
                return HabBot.State.Craft;
            case Type.BuildingUnfinished:
                return HabBot.State.Build;
            case Type.Mineral:
                return HabBot.State.CollectMinerals;
            case Type.Food:
                return HabBot.State.CollectFood;
            case Type.Tree:
                return HabBot.State.CollectTrees;
        }
        return HabBot.State.Idle;
    }
}


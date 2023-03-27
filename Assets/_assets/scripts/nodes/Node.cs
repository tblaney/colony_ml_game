using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Node 
{
    public int _prefab;
    public int _inventoryIndex = 0;
    public Vector3Int _position;
    public bool _active = true;
    public bool _surface;
    public bool _busy;
    public float _timer;
    public float _refreshRate;
    public float _propogationFactor = 0f; 
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
    public Type _type;

    public Node(int prefab, Vector3Int position, Type type)
    {
        _prefab = prefab;
        _position = position;
        _type = type;
        _active = true;
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
        for (int i = 0; i < 100; i++)
        {
            Vector3Int position = _position + new Vector3Int(0, i, 0);
            Node node = NodeProcessor._nodeActions.GetNodeFunc(position);
            if (node != null)
            {
                nodes.Add(node);
            } else
            {
                break;
            }
        }
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
}


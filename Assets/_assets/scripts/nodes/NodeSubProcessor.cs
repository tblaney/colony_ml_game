using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeSubProcessor : MonoBehaviour
{
    // runtime class
    public int _index;
    public Bounds _bounds;
    public Bounds _boundsLoad;
    public Vector2Int _positionGrid;
    public bool _active;
    Dictionary<Node.Type, Dictionary<Vector3Int, Node>> _nodes;
    Dictionary<Node.Type, List<NodeObject>> _nodeObjects;
    bool _loading = false;
    Coroutine _loadingRoutine;

    //___static___//
    public static NodeSubProcessor SpawnNodeSubProcessor(Transform parent)
    {
        GameObject obj = new GameObject(){};
        obj.name = "Node Sub Processor";
        obj.transform.SetParent(parent);
        NodeSubProcessor processor = obj.AddComponent<NodeSubProcessor>();
        return processor;
    }
    //___setup___//
    public void Initialize(int index, Bounds bounds, Vector2Int positionGrid)
    {   
        _index = index;
        _bounds = bounds;

        _boundsLoad = new Bounds();
        _boundsLoad.center = bounds.center;
        _boundsLoad.size = new Vector3(200f, 30f, 200f);

        _positionGrid = positionGrid;

        _nodes = new Dictionary<Node.Type, Dictionary<Vector3Int, Node>>();
        _nodeObjects = new Dictionary<Node.Type, List<NodeObject>>();

        Load(false);
    }
    //___load&unload___//
    public void Load(bool active = true)
    {
        _nodeObjects = new Dictionary<Node.Type, List<NodeObject>>();
        if (active)
        {
            _loadingRoutine = StartCoroutine(LoadRoutine());
            //foreach (Node node in GetNodes())
            //{
            //    SpawnNode(node);
            //}
        } else
        {
            foreach (Node node in GetNodes())
            {
                UnloadNode(node);
            }
        }
        _active = active;
    }
    public void LoadCheck(Vector3 position)
    {
        if (_loading)
            return;
        
        if (_boundsLoad.Contains(position))
        {
            if (!_active)
            {
                Load(true);
            }
        } else
        {
            if (_active)
            {
                Load(false);
            }
        }
    }
    void SpawnNode(Node node)
    {
        GameObject obj = Instantiate(PrefabHandler.Instance.GetPrefab(node._prefab), node._position, Quaternion.identity, this.transform);
        NodeObject nodeObject = obj.GetComponent<NodeObject>();
        nodeObject.Initialize(node, DestroyNodeCallback);
        List<NodeObject> objs;
        if (_nodeObjects.TryGetValue(node._nodeType, out objs))
        {
            objs.Add(nodeObject);
        } else
        {
            _nodeObjects.Add(node._nodeType, new List<NodeObject>(){nodeObject});
        }
    }
    void DestroyNodeCallback(Node node)
    {
        // need to clear from storage
        // not to be called when load/unload
        RemoveNode(node);
        NodeObject obj = GetNodeObject(node);
        if (obj != null)
        {
            List<NodeObject> objs;
            if (_nodeObjects.TryGetValue(node._nodeType, out objs))
            {
                if (objs.Contains(obj))
                {
                    objs.Remove(obj);
                }
            }
        }
    }
    void UnloadNode(Node node)
    {
        // unload physical representation of node
        NodeObject obj = GetNodeObject(node);
        if (obj != null)
        {
            Destroy(obj.gameObject);
        }
    }
    //___bounds___//
    public bool IsWithin(Vector3 position)
    {
        return _bounds.Contains(position);
    }
    //___gets___//
    public Node GetNode(Vector3Int position)
    {
        if (_nodes == null)
            return null;

        foreach (Dictionary<Vector3Int, Node> dic in _nodes.Values)
        {
            Node node;
            if (dic.TryGetValue(position, out node))
                return node;
        }
        return null;
    }
    public List<Node> GetNodes()
    {
        List<Node> nodes = new List<Node>();
        if (_nodes == null)
            return nodes;
        foreach (Dictionary<Vector3Int, Node> dic in _nodes.Values)
        {
            foreach (Node obj in dic.Values)
            {   
                nodes.Add(obj);
            }
        }
        return nodes;
    }
    public List<Node> GetNodes(Node.Type type)
    {
        List<Node> nodes = new List<Node>();
        Dictionary<Vector3Int, Node> dic;
        if (_nodes.TryGetValue(type, out dic))
        {
            nodes.AddRange(dic.Values);
        }
        return nodes;
    }
    public Node GetClosestNodeOfType(Node.Type type, Vector3 position, int prefabIndex = 0)
    {
        Dictionary<Vector3Int, Node> dic;
        if (_nodes.TryGetValue(type, out dic))
        {
            float distanceMin = 10000f;
            Node closest = null;
            foreach (Node node in dic.Values)
            {
                if (node._busy)
                    continue;
                
                if (prefabIndex != 0)
                {
                    if (node._prefab != prefabIndex)
                        continue;
                }
                float distance = Vector3.Distance(position, node._position);
                if (distance < distanceMin)
                {
                    closest = node;
                    distanceMin = distance;
                }
            }
            return closest;
        }
        return null;
    }
    public NodeObject GetNodeObject(Node node)
    {
        List<NodeObject> objs;
        if (_nodeObjects.TryGetValue(node._nodeType, out objs))
        {
            foreach (NodeObject obj in objs)
            {
                if (obj._node == node)
                    return obj;
            }
        }
        Debug.Log("No Node Object Found");
        return null;
    }
    public List<NodeObject> GetNodeObjects()
    {
        List<NodeObject> returnObjs = new List<NodeObject>();
        foreach (List<NodeObject> objs in _nodeObjects.Values)
        {
            returnObjs.AddRange(objs);
        }
        return returnObjs;
    }
    //___nodes___//
    public void AddNode(Node node)
    {
        if (_nodes == null)
            _nodes = new Dictionary<Node.Type, Dictionary<Vector3Int, Node>>();

        Dictionary<Vector3Int, Node> dic;
        if (_nodes.TryGetValue(node._nodeType, out dic))
        {
            dic.Add(node._position, node);
        } else
        {
            dic = new Dictionary<Vector3Int, Node>();
            dic.Add(node._position, node);
            _nodes.Add(node._nodeType, dic);
        }
        if (_active)
        {
            SpawnNode(node);
        }
    }
    public void RemoveNode(Node node)
    {
        if (_nodes == null)
            return;

        Dictionary<Vector3Int, Node> dic;
        if (_nodes.TryGetValue(node._nodeType, out dic))
        {
            Node nodeTemp;
            if (dic.TryGetValue(node._position, out nodeTemp))
            {
                dic.Remove(nodeTemp._position);
            }
        }
    }
    public void RefreshPropagation(Node.Type type)
    {
        Dictionary<Vector3Int, Node> dic;
        if (_nodes.TryGetValue(type, out dic))
        {
            foreach (Node node in dic.Values)
            {
                node.TryPropagate();
            }
        }
    }
    private IEnumerator LoadRoutine()
    {
        _loading = true;
        int i = 0;
        foreach (Node node in GetNodes())
        {
            SpawnNode(node);
            i++;
            if (i % 15 == 0)
                yield return null;
        }
        _loading = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeProcessor : MonoBehaviour
{
    [Header("Input:")]
    public List<NodeGroup> _groups;
    public Bounds _bounds;
    public Vector2Int _subProcessorAmount;
    public static float _boundsHeight;

    [Header("Debug:")]
    public List<NodeSubProcessor> _processors;
    NodeProcessorCaster _caster;


    // setup
    public void Initialize()
    {
        // spinup sub processors with bounds:
        _caster = GetComponent<NodeProcessorCaster>();
        InitializeSubProcessors();
        _boundsHeight = _bounds.center.y;
    }
    void InitializeSubProcessors()
    {
        _processors = new List<NodeSubProcessor>();
        List<Bounds> boundsSplit = Utils.Tools.SplitBounds(_bounds, _subProcessorAmount.x, _subProcessorAmount.y);
        float intervalX = _bounds.size.x/_subProcessorAmount.x;
        float intervalZ = _bounds.size.z/_subProcessorAmount.y;
        int i = 1;
        foreach (Bounds bounds in boundsSplit)
        {
            NodeSubProcessor processor = new NodeSubProcessor(){};
            processor._positionGrid = new Vector2Int((int)((bounds.min.x - 300f)/intervalX), (int)((bounds.min.z - 300f)/intervalZ));
            processor._bounds = bounds;
            processor._index = i;
            _processors.Add(processor);
            i++;
        }
    }
    public void Load(List<Node> nodes)
    {
        if (nodes == null)
        {   
            foreach (NodeGroup group in _groups)
            {
                if (group._cluster)
                {

                    int clusterAmount = (int)(group._amount/UnityEngine.Random.Range(5, 12));
                    for (int i = 0; i < group._amount; i++)
                    {
                        List<NodeObject> spawnedObjects = new List<NodeObject>();
                        spawnedObjects = ClusterSpawnNode(group, clusterAmount);
                        i += spawnedObjects.Count - 1;
                    }
                } else
                {
                    for (int i = 0; i < group._amount; i++)
                    {
                        Vector3Int position = GetOpenPosition();
                        if (position == default(Vector3Int))
                            continue;
                        
                        Node node = new Node(group.GetRandomPrefab()){_position = position};
                        SpawnNode(node);
                    }
                }
            }
        } else
        {
            foreach (Node node in nodes)
            {
                SpawnNode(node);
            }
        }
        // surface check:
        foreach (NodeSubProcessor processor in _processors)
        {
            List<NodeObject> nodeObjects = processor.GetNodeObjects();
            foreach (NodeObject node in nodeObjects)
            {
                node.SurfaceCheck();
            }
        }
    }
    
    // node actions
    public NodeObject SpawnNode(Node node)
    {
        GameObject obj = Instantiate(PrefabHandler.Instance.GetPrefab(node._prefab), node._position, Quaternion.identity, this.transform);
        NodeObject nodeObject = obj.GetComponent<NodeObject>();
        nodeObject.Initialize(node, DestroyNode, GetNodeNeighbours);
        NodeSubProcessor processor  = GetSubProcessor(node._position);
        processor.AddNodeObject(nodeObject);
        return nodeObject;
    }
    void DestroyNode(NodeObject nodeObject)
    {
        NodeSubProcessor processor  = GetSubProcessor(nodeObject.GetPosition());
        processor.RemoveNodeObject(nodeObject);
    }
    List<NodeObject> ClusterSpawnNode(NodeGroup group, int amount)
    {
         // this will spawn in other nodes of the same object around it randomly
        List<NodeObject> nodeObjects = new List<NodeObject>();
        Vector3Int position = GetOpenPosition();
        if (position == default(Vector3Int))
            return nodeObjects;
        
        for (int i = 0; i < amount; i++)
        {
            Node node = new Node(group.GetRandomPrefab()){_position = position};
            NodeObject nodeObject = SpawnNode(node);
            nodeObjects.Add(nodeObject);
            nodeObjects.AddRange(AddHeight(group, nodeObject));

            List<Vector3Int> newPositions = GetOpenNeighbours(nodeObject);
            if (newPositions.Count > 0)
            {
                position = newPositions[UnityEngine.Random.Range(0, newPositions.Count)];
            } else
            {
                break;
            }
        }
        return nodeObjects;
    }
    List<NodeObject> AddHeight(NodeGroup group, NodeObject nodeObject)
    {
        List<NodeObject> nodeObjects = new List<NodeObject>();
        float freq = 2004f;
        float x = nodeObject.GetPosition().x;
        float z = nodeObject.GetPosition().z;
        float val = Mathf.PerlinNoise((float)freq*(x)/_bounds.size.x, (float)freq*(z)/_bounds.size.z);
        Debug.Log("Perlin Noise Val: " + val);
        int amount = (int)(val*group._clusterHeight);
        for (int i = 1; i <= amount; i++)
        {
            Vector3Int position = nodeObject.GetPosition() + new Vector3Int(0, 1, 0)*i;
            Node node = new Node(group.GetRandomPrefab()){_position = position};
            NodeObject nodeObjectNew = SpawnNode(node);
            nodeObjects.Add(nodeObjectNew);
        }
        return nodeObjects;
    }
    List<Node> CollectNodes()
    {
        List<Node> nodes = new List<Node>();
        foreach (NodeSubProcessor processor in _processors)
        {
            nodes.AddRange(processor.GetNodes());
        }
        return nodes;
    }

    // gets
    Vector3Int GetOpenPosition()
    {
        // try for 30 iterations
        for (int i = 0; i < 30; i++)
        {
            Vector3Int position = new Vector3Int(UnityEngine.Random.Range((int)_bounds.min.x, (int)_bounds.max.x), (int)_bounds.center.y, UnityEngine.Random.Range((int)_bounds.min.z, (int)_bounds.max.z));
            NodeSubProcessor subProcessor = GetSubProcessor(position);
            if (subProcessor != null)
            {
                NodeObject nodeObject = subProcessor.GetNodeObject(position);
                if (nodeObject == null)
                {
                    // need to also cast
                    RaycastHit[] hits = _caster.BlockerCast(position);
                    if (hits.Length == 0)
                        return position;
                }
            } 
        }
        return default(Vector3Int);
    }
    NodeSubProcessor GetSubProcessor(Vector3 position)
    {
        foreach (NodeSubProcessor processor in _processors)
        {
            if (processor.IsWithin(position))
            {
                return processor;
            }
        }
        return null;
    }
    NodeSubProcessor GetSubProcessorByGrid(Vector2Int gridPosition)
    {
        foreach (NodeSubProcessor processor in _processors)
        {
            if (processor._positionGrid == gridPosition)
                return processor;
        }
        return null;
    }
    public NodeObject GetNodeObject(Vector3Int position)
    {
        NodeSubProcessor processor = GetSubProcessor(position);
        if (processor == null)
            return null;
        return processor.GetNodeObject(position);
    }
    public List<NodeObject> GetNodeNeighbours(NodeObject nodeObject, bool is4 = true)
    {
        Vector3Int position = nodeObject.GetPosition();
        Vector3Int p1 = position + new Vector3Int(1, 0, 0);
        Vector3Int p2 = position + new Vector3Int(-1, 0, 0);
        Vector3Int p3 = position + new Vector3Int(0, 0, 1);
        Vector3Int p4 = position + new Vector3Int(0, 0, -1);
        List<Vector3Int> positions = new List<Vector3Int>() {p1, p2, p3, p4};
        if (!is4)
        {
            // 8 neighbours
            Vector3Int p5 = position + new Vector3Int(1, 0, 1);
            Vector3Int p6 = position + new Vector3Int(1, 0, -1);
            Vector3Int p7 = position + new Vector3Int(-1, 0, 1);
            Vector3Int p8 = position + new Vector3Int(-1, 0, -1);
            positions.Add(p5);
            positions.Add(p6);
            positions.Add(p7);
            positions.Add(p8);
        }
        List<NodeObject> nodes = new List<NodeObject>();
        foreach (Vector3Int pos in positions)
        {
            NodeObject obj = GetNodeObject(pos);
            if (obj != null)
            {
                nodes.Add(obj);
            }
        }
        return nodes;
    }
    List<Vector3Int> GetOpenNeighbours(NodeObject nodeObject)
    {
        Vector3Int position = nodeObject.GetPosition();
        Vector3Int p1 = position + new Vector3Int(1, 0, 0);
        Vector3Int p2 = position + new Vector3Int(-1, 0, 0);
        Vector3Int p3 = position + new Vector3Int(0, 0, 1);
        Vector3Int p4 = position + new Vector3Int(0, 0, -1);
        List<Vector3Int> positions = new List<Vector3Int>() {p1, p2, p3, p4};
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (Vector3Int pos in positions)
        {
            NodeSubProcessor processor = GetSubProcessor(pos);
            if (processor == null)
                continue;
            
            NodeObject obj = GetNodeObject(pos);
            if (obj == null)
            {
                RaycastHit[] hits = _caster.BlockerCast(pos);
                if (hits.Length == 0)
                    neighbours.Add(pos);
            }
        }
        return neighbours;
    }
    public NodeGroup GetNodeGroup(int index)
    {
        foreach (NodeGroup group in _groups)
        {
            if (group._index == index)
                return group;
        }
        return null;
    }
    public List<Node> GetNodes()
    {
        List<Node> nodes = CollectNodes();
        return nodes;
    }
    public NodeObject GetClosestNodeObject(Node.Type _type, Vector3 position)
    {
        // order sub processors:
        List<NodeSubProcessor> processorsOrders = GetOrdereredSubProcessors(position);
        foreach (NodeSubProcessor processor in processorsOrders)
        {
            NodeObject node = processor.GetClosestNodeObjectOfType(_type, position);
            if (node != null)
            {
                return node;
            }
        }
        return null;
    }
    public List<NodeSubProcessor> GetOrdereredSubProcessors(Vector3 position)
    {
        List<NodeSubProcessor> processorsSorted = new List<NodeSubProcessor>();
        foreach (NodeSubProcessor processor in _processors)
        {
            if (processorsSorted.Count == 0)
            {
                processorsSorted.Add(processor);
            }
            else
            {
                float distance = Vector3.Distance(position, processor._bounds.center);
                int i = 0;
                foreach (NodeSubProcessor processorTemp in processorsSorted)
                {
                    float distanceTemp = Vector3.Distance(position, processorTemp._bounds.center);
                    if (distance < distanceTemp)
                    {
                        processorsSorted.Insert(i, processor);
                        break;
                    }
                    i++;
                }
            }   
        }
        return processorsSorted;
    }
}

[Serializable]
public class NodeSubProcessor
{
    // runtime class
    public int _index;
    public Bounds _bounds;
    public Dictionary<Node.Type, Dictionary<Vector3Int, NodeObject>> _nodes;
    public Vector2Int _positionGrid;

    public bool IsWithin(Vector3 position)
    {
        return _bounds.Contains(position);
    }
    public NodeObject GetNodeObject(Vector3Int position)
    {
        if (_nodes == null)
            return null;

        foreach (Dictionary<Vector3Int, NodeObject> dic in _nodes.Values)
        {
            NodeObject node;
            if (dic.TryGetValue(position, out node))
                return node;
        }
        return null;
    }
    public void AddNodeObject(NodeObject nodeObject)
    {
        if (_nodes == null)
            _nodes = new Dictionary<Node.Type, Dictionary<Vector3Int, NodeObject>>();

        Dictionary<Vector3Int, NodeObject> dic;
        if (_nodes.TryGetValue(nodeObject.GetNode()._type, out dic))
        {
            dic.Add(nodeObject.GetPosition(), nodeObject);
        } else
        {
            dic = new Dictionary<Vector3Int, NodeObject>();
            dic.Add(nodeObject.GetPosition(), nodeObject);
            _nodes.Add(nodeObject.GetNode()._type, dic);
        }
        //_nodes.Add(nodeObject.GetPosition(), nodeObject);
    }
    public void RemoveNodeObject(NodeObject nodeObject)
    {
        if (_nodes == null)
            return;

        Dictionary<Vector3Int, NodeObject> dic;
        if (_nodes.TryGetValue(nodeObject.GetNode()._type, out dic))
        {
            dic.Remove(nodeObject.GetPosition());
        }
    }
    public List<Node> GetNodes()
    {
        List<Node> nodes = new List<Node>();
        foreach (Dictionary<Vector3Int, NodeObject> dic in _nodes.Values)
        {
            foreach (NodeObject obj in dic.Values)
            {   
                nodes.Add(obj.GetNode());
            }
        }
        return nodes;
    }
    public List<NodeObject> GetNodeObjects()
    {
        List<NodeObject> nodes = new List<NodeObject>();
        if (_nodes == null)
            return nodes;
        foreach (Dictionary<Vector3Int, NodeObject> dic in _nodes.Values)
        {
            foreach (NodeObject obj in dic.Values)
            {   
                nodes.Add(obj);
            }
        }
        return nodes;
    }
    public NodeObject GetClosestNodeObjectOfType(Node.Type _type, Vector3 position)
    {
        Dictionary<Vector3Int, NodeObject> dic;
        if (_nodes.TryGetValue(_type, out dic))
        {
            float distanceMin = 10000f;
            NodeObject closest = null;
            foreach (NodeObject node in dic.Values)
            {
                float distance = Vector3.Distance(position, node.GetPosition());
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
}

[Serializable]
public class NodeGroup
{
    [Header("Inputs:")]
    public string _name;
    public int _index;

    [Space(10)]
    [Tooltip("Link to Prefab Handler and Define Distribution")]
    public List<NodeGroupPrefab> _prefabs;

    [Space(10)]
    public int _amount;
    public bool _cluster = false;
    public int _clusterHeight = 8;
    //public int _clusterAmount = 18;

    [Space(10)]
    public float _refreshRate = 0f;

    public int GetRandomPrefab()
    {
        float val = UnityEngine.Random.Range(0f, 1f);
        foreach (NodeGroupPrefab prefab in _prefabs)
        {
            if (prefab.InRange(val))
                return prefab._prefabIndex;
        }
        return 0;
    }
}

[Serializable]
public class NodeGroupPrefab
{
    public float[] _probabilityRange;
    public int _prefabIndex;

    public bool InRange(float val)
    {
        if (val >= _probabilityRange[0] && val < _probabilityRange[1])
            return true;
        return false;
    }
}

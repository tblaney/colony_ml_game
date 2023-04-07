using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeProcessor : MonoBehaviour
{
    [Header("Input:")]
    public List<NodeGroup> _groups;
    public List<NodeObject> _objectsToSave;
    public Bounds _bounds;
    public Vector2Int _subProcessorAmount;
    public static float _boundsHeight;
    [SerializeField] private Transform _followTransform;
    public bool _performProcessing;

    [Header("Debug:")]
    public List<NodeSubProcessor> _processors;
    NodeProcessorCaster _caster;
    public static NodeActions _nodeActions;
    public static NodeStatus _nodeStatus;
    public static event EventHandler OnProcessorReady;

    public bool _checkedActive = false;


    void OnDisable()
    {
        TimeHandler._timeWorld.OnMidnight -= Time_OnMidnight;
    }
    void Time_OnMidnight(object sender, EventArgs e)
    {
        RefreshNodeStatus();
        RefreshPropagation();
    }
    // setup
    public void Initialize()
    {
        // spinup sub processors with bounds:
        _caster = GetComponent<NodeProcessorCaster>();
        InitializeSubProcessors();
        _boundsHeight = _bounds.center.y;
        foreach (NodeObject obj in _objectsToSave)
        {
            Debug.Log("Node Processor: " + AssignNode(obj.GetNode()));
            Destroy(obj.gameObject);
        }
        _objectsToSave.Clear();
        _nodeActions = new NodeActions()
        {
            GetNeighboursFunc = GetNodeNeighbours,
            GetNodeFunc = GetNode,
            GetOpenNeighboursFunc = GetOpenNeighbours,
        };
        _nodeStatus = new NodeStatus();
        TimeHandler._timeWorld.OnMidnight += Time_OnMidnight;
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
            NodeSubProcessor processor = NodeSubProcessor.SpawnNodeSubProcessor(this.transform);
            Vector2Int positionGrid = new Vector2Int((int)((bounds.min.x - 300f)/intervalX), (int)((bounds.min.z - 300f)/intervalZ));
            processor.Initialize(i, bounds, positionGrid);
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
                        List<Node> spawnedObjects = new List<Node>();
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
                        Node node = new Node(group.GetRandomPrefab(), position, group._type, group._health){_propogationFactor = group._propogationFactor};
                        AssignNode(node);
                    }
                }
            }
        } else
        {
            foreach (NodeSubProcessor processor in _processors)
            {
                processor.Reset();
            }
            foreach (Node node in nodes)
            {
                AssignNode(node);
            }
        }
        if (!_performProcessing)
        {
            foreach (NodeSubProcessor processor in _processors)
            {
                processor.Load(true);
            }
        }
        RefreshNodeStatus();
    }
    void Update()
    {
        if (_processors == null)
            return;
        if (!_checkedActive)
        {
            bool passed = true;
            foreach (NodeSubProcessor processor in _processors)
            {
                if (!processor._active)
                {
                    Debug.Log("Node Processor Fail: " + processor._index);
                    passed = false;
                    break;
                }
            }
            if (passed)
            {
                Debug.Log("All Processors Active");
                _checkedActive = true;
                OnProcessorReady?.Invoke(this, EventArgs.Empty);
            } 
        }
        if (!_performProcessing)
            return;
        foreach (NodeSubProcessor processor in _processors)
        {
            processor.LoadCheck(_followTransform.position);
        }
    }
    // node actions
    public bool AssignNode(Node node)
    {
        // assign it to a sub-processor
        foreach (NodeSubProcessor processor in _processors)
        {
            if (processor.IsWithin(node._position))
            {
                processor.AddNode(node);
                return true;
            }
        }
        return false;
    }
    public NodeObject SpawNode(Node node)
    {
        GameObject obj = Instantiate(PrefabHandler.Instance.GetPrefab(node._prefab), node._position, Quaternion.identity, this.transform);
        NodeObject nodeObject = obj.GetComponent<NodeObject>();
        nodeObject.Initialize(node, null);
        return nodeObject;
    }
    List<Node> ClusterSpawnNode(NodeGroup group, int amount)
    {
         // this will spawn in other nodes of the same object around it randomly
        List<Node> nodes = new List<Node>();
        Vector3Int position = GetOpenPosition();
        if (position == default(Vector3Int))
            return nodes;
        
        for (int i = 0; i < amount; i++)
        {
            Node node = new Node(group.GetRandomPrefab(), position, group._type, group._health){_propogationFactor = group._propogationFactor};
            AssignNode(node);
            nodes.Add(node);
            nodes.AddRange(AddHeight(group, node));

            List<Vector3Int> newPositions = GetOpenNeighbours(node);
            if (newPositions.Count > 0)
            {
                position = newPositions[UnityEngine.Random.Range(0, newPositions.Count)];
            } else
            {
                break;
            }
        }
        return nodes;
    }
    List<Node> AddHeight(NodeGroup group, Node node)
    {
        List<Node> nodes = new List<Node>();
        float freq = 2004f;
        float x = node._position.x;
        float z = node._position.z;
        float val = Mathf.PerlinNoise((float)freq*(x)/_bounds.size.x, (float)freq*(z)/_bounds.size.z);
        //Debug.Log("Perlin Noise Val: " + val);
        int amount = (int)(val*group._clusterHeight);
        for (int i = 1; i <= amount; i++)
        {
            Vector3Int position = node._position + new Vector3Int(0, 1, 0)*i;
            Node nodeTemp = new Node(group.GetRandomPrefab(), position, group._type, group._health){_propogationFactor = group._propogationFactor};
            AssignNode(nodeTemp);
            nodes.Add(nodeTemp);
        }
        return nodes;
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
    // refresh
    public void RefreshPropagation()
    {
        foreach (NodeGroup group in _groups)
        {
            
        }
    }
    public void RefreshNodeStatus()
    {   
        _nodeStatus.Clear();
        List<Node> nodesAll = GetAllNodes();
        foreach (Node.Type type in Enum.GetValues(typeof(Node.Type)))
        {
            _nodeStatus._dicAmount.Add(type, 0);
        }
        foreach (Node node in nodesAll)
        {
            _nodeStatus._dicAmount[node._nodeType]++;
        }
        foreach (Node.Type type in _nodeStatus._dicAmount.Keys)
        {
            Debug.Log("Refresh Node Status; " + type + ", " + _nodeStatus._dicAmount[type]);
        }
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
                Node node = subProcessor.GetNode(position);
                if (node == null)
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
    public Vector3Int GetOpenPositionBounds(Bounds bounds)
    {
        // try for 30 iterations
        for (int i = 0; i < 30; i++)
        {
            Vector3Int position = new Vector3Int(UnityEngine.Random.Range((int)bounds.min.x, (int)bounds.max.x), (int)bounds.center.y, UnityEngine.Random.Range((int)bounds.min.z, (int)bounds.max.z));
            NodeSubProcessor subProcessor = GetSubProcessor(position);
            if (subProcessor != null)
            {
                Node node = subProcessor.GetNode(position);
                if (node == null)
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
    public List<Node> GetNodeNeighbours(Node nodeObject, bool is4 = true)
    {
        Vector3Int position = nodeObject._position;
        Vector3Int p1 = position + new Vector3Int(1, 0, 0);
        Vector3Int p2 = position + new Vector3Int(-1, 0, 0);
        Vector3Int p3 = position + new Vector3Int(0, 0, 1);
        Vector3Int p4 = position + new Vector3Int(0, 0, -1);
        List<Vector3Int> positions = new List<Vector3Int>() {p1, p2, p3, p4};
        if (!is4)
        {
            // 8 neighbours
            Vector3Int p5 = position + new Vector3Int(0, 1, 0);
            Vector3Int p6 = position + new Vector3Int(0, -1, 0);
            positions.Add(p5);
            positions.Add(p6);
        }
        List<Node> nodes = new List<Node>();
        foreach (Vector3Int pos in positions)
        {
            Node obj = GetNode(pos);
            if (obj != null)
            {
                nodes.Add(obj);
            }
        }
        return nodes;
    }
    List<Vector3Int> GetOpenNeighbours(Node node)
    {
        Vector3Int position = node._position;
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
            
            Node obj = GetNode(pos);
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
    public List<Node> GetAllNodes()
    {
        List<Node> nodes = CollectNodes();
        return nodes;
    }
    public Node GetNode(Vector3Int position)
    {
        NodeSubProcessor processor = GetSubProcessor(position);
        if (processor == null)
            return null;
        
        Node node = processor.GetNode(position);
        if (node != null)
            return node;
        
        return null;
    }
    public NodeObject GetClosestNodeObject(Node.Type type, Vector3 position, int prefabIndex = 0)
    {
        // order sub processors:
        List<NodeSubProcessor> processorsOrders = GetOrdereredSubProcessors(position);
        foreach (NodeSubProcessor processor in processorsOrders)
        {
            Node node = processor.GetClosestNodeOfType(type, position, prefabIndex);
            if (node != null)
            {
                Debug.Log("Found Node: " + node._prefab);
                return processor.GetNodeObject(node);
            }
        }
        return null;
    }
    public NodeObject GetNodeObject(Node node)
    {
        NodeSubProcessor processor = GetSubProcessor(node._position);
        if (processor != null)
        {
            return processor.GetNodeObject(node);
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
                bool foundSpot = false;
                foreach (NodeSubProcessor processorTemp in processorsSorted)
                {
                    float distanceTemp = Vector3.Distance(position, processorTemp._bounds.center);
                    if (distance < distanceTemp)
                    {
                        processorsSorted.Insert(i, processor);
                        foundSpot = true;
                        break;
                    }
                    i++;
                }
                if (!foundSpot)
                {
                    processorsSorted.Add(processor);
                }
            }   
        }
        return processorsSorted;
    }
    public List<Node> GetNodesByType(Node.Type type)
    {
        List<Node> nodes = new List<Node>();
        foreach (NodeSubProcessor processor in _processors)
        {
            nodes.AddRange(processor.GetNodes(type));
        }
        return nodes;
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
    public Node.Type _type;

    [Space(10)]
    public int _amount;
    public bool _cluster = false;
    public int _clusterHeight = 8;
    public int _health = 100;
    //public int _clusterAmount = 18;

    [Space(10)]
    public float _propogationFactor = 0f;

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

public class NodeActions
{
    public Func<Node, bool, List<Node>> GetNeighboursFunc;
    public Func<Vector3Int, Node> GetNodeFunc;
    public Func<Node, List<Vector3Int>> GetOpenNeighboursFunc;
}

public class NodeStatus
{
    public Dictionary<Node.Type, int> _dicAmount;
    public Dictionary<Node.Type, int> _dixMax;
    public void Clear()
    {
        _dicAmount = new Dictionary<Node.Type, int>();
    }
}
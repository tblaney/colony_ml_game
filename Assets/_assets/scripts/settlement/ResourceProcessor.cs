using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceProcessor : MonoBehaviour
{
    /*
        Handles all spawning/despawning of in-game objects like food/minerals
    */
    [Header("Inputs:")]
    public List<Processor> processors;
    public MeshRenderer plane;
    public Bounds bounds;

    public void Initialize()
    {   
        bounds = plane.bounds;
        bounds.size = new Vector3(bounds.size.x, 5f, bounds.size.z);
        ProcessorSetup();
    }
    void ProcessorSetup()
    {
        foreach (Processor processor in processors)
        {
            processor.Initialize();
        }
    }
    public void Load(List<NodeSave> saves)
    {
        if (saves == null)
        {
            Reset();
        } else
        {

        }
    }    
    public void Reset()
    {
        ClearAll();
        foreach (Processor processor in processors)
        {
            for (int i = 0; i < processor.amount; i++)
            {
                Spawn(processor);
            }
        }
        foreach (Processor processor in processors)
        {
            foreach (Node node in processor.nodesList)
            {
                node.SurfaceCheck();
            }
        }
    }
    public void RefreshInactive()
    {
        foreach (Processor processor in processors)
        {
            if (!processor.refreshable)
                continue;
            
            foreach (Node node in processor.nodes.Values)
            {
                node.InactiveCheck();
            }
        }
    }
    public void ClearAll()
    {
        foreach (Processor processor in processors)
        {
            if (processor.nodes == null || processor.nodes.Count == 0)
                continue;
            
            foreach (Node node in processor.nodes.Values)
            {
                Destroy(node.GetGameObject());
            }

            processor.nodes.Clear();
        }
    }

    public void Spawn(Processor processor)
    {
        GameObject obj = Instantiate(processor.prefab, this.transform);
        Vector3 pos = GetOpenPosition();
        Vector3Int key = new Vector3Int((int)pos.x, (int)plane.transform.position.y, (int)pos.z);
        obj.transform.position = key;
        Node node = obj.transform.GetChild(0).GetComponent<Node>();
        node.Initialize(key, GetNode, NodeDestroyCallback, NodeActivateCallback, processor.name, processor.refreshable, plane.transform.position.y);
        processor.AddNode(node);
        if (processor.clusterAmounts != null && processor.clusterAmounts.Length > 0)
        {
            List<Node> clusteredNodes = ClusterNode(node, processor.clusterAmounts[0] + UnityEngine.Random.Range(-8, 8), processor);
            int i = 0;
            List<Node> newClusteredNodes = new List<Node>();
            foreach (int j in processor.clusterAmounts)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                newClusteredNodes.Clear();
                for (int k = 0; k < j; k++)
                {
                    if (k >= clusteredNodes.Count)
                        continue;
                    
                    newClusteredNodes.AddRange(ClusterNodeTop(clusteredNodes[k], 1, processor));
                }
                clusteredNodes = new List<Node>(newClusteredNodes);
                i++;
            }
        }
    }

    public void SpawnProcessorObject(string name)
    {
        // spawns an object of the right type
        Processor processor = GetProcessor(name);
        if (processor != null)
        {
            Spawn(processor);
        }
    }
    List<Node> ClusterNode(Node nodeIn, int amount, Processor processor)
    {
        // this will spawn in other nodes of the same object around it randomly
        List<Node> nodes = new List<Node>();
        for (int i = 0; i < amount; i++)
        {
            List<Vector3Int> neighbours = GetOpenNeighbours(nodeIn);
            Debug.Log("Cluster node: " + neighbours.Count);
            if (neighbours.Count > 0)
            {
                Vector3Int neighbourPosition = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
                GameObject newObj = Instantiate(nodeIn.transform.parent, this.transform).gameObject;
                newObj.transform.position = neighbourPosition;
                Node node = newObj.transform.GetChild(0).GetComponent<Node>();
                node.Initialize(neighbourPosition, GetNode, NodeDestroyCallback, NodeActivateCallback, processor.name, processor.refreshable, plane.transform.position.y);
                nodes.Add(node);
                nodeIn = node;
                processor.AddNode(node);
            } else
            {
                break;
            }
        }
        return nodes;
    }
    List<Node> ClusterNodeTop(Node nodeIn, int amount, Processor processor)
    {
        List<Node> nodes = new List<Node>();
        GameObject newObj = Instantiate(nodeIn.transform.parent, this.transform).gameObject;
        Vector3Int position = nodeIn.GetPosition() + new Vector3Int(0, 1, 0);
        newObj.transform.position = position;
        Node node = newObj.transform.GetChild(0).GetComponent<Node>();
        node.Initialize(position, GetNode, NodeDestroyCallback, NodeActivateCallback, processor.name, processor.refreshable, plane.transform.position.y);
        nodes.Add(node);
        processor.AddNode(node);
        Debug.Log("Cluster Node Top: " + node.GetPosition());
        return nodes;
    }
    void NodeDestroyCallback(Node node)
    {
        // need to remove from collections in processorobject and processornode
        foreach (Processor processor in processors)
        {
            processor.RemoveNode(node);
        }
    }
    void NodeActivateCallback(Node node)
    {
        Processor processor = GetProcessor(node.save.nameProcessor);
        if (node.save.active)
        {
            processor.AddActiveNode(node);
        } else
        {
            processor.RemoveActiveNode(node);
        }
    }
    Vector3 GetOpenPosition()
    {
        // try for 30 iterations
        for (int i = 0; i < 30; i++)
        {
            Vector3Int position = new Vector3Int(UnityEngine.Random.Range((int)bounds.min.x, (int)bounds.max.x), (int)plane.transform.position.y, UnityEngine.Random.Range((int)bounds.min.z, (int)bounds.max.z));
            bool success = true;
            foreach (Processor processor in processors)
            {
                if (processor.GetNode(position) != null)
                {
                    success = false;
                    break;   
                }
            }
            if (success)
            {
                return position;
                break;
            }
        }
        return default(Vector3);
    }
    List<Vector3Int> GetOpenNeighbours(Node nodeIn)
    {
        Vector3Int position = nodeIn.GetPosition();
        Vector3Int p1 = position + new Vector3Int(1, 0, 0);
        Vector3Int p2 = position + new Vector3Int(-1, 0, 0);
        Vector3Int p3 = position + new Vector3Int(0, 0, 1);
        Vector3Int p4 = position + new Vector3Int(0, 0, -1);
        Vector3Int p5 = position + new Vector3Int(1, 0, 1);
        Vector3Int p6 = position + new Vector3Int(1, 0, -1);
        Vector3Int p7 = position + new Vector3Int(-1, 0, 1);
        Vector3Int p8 = position + new Vector3Int(-1, 0, -1);
        List<Vector3Int> positions = new List<Vector3Int>() {p1, p2, p3, p4, p5, p6, p7, p8};
        List<Vector3Int> nodes = new List<Vector3Int>();
        foreach (Vector3Int pos in positions)
        {
            Vector3 positionFixed = new Vector3(pos.x, nodeIn.transform.position.y, pos.z);
            if (!bounds.Contains(positionFixed))
                continue;
            
            bool success = true;
            foreach (Processor processor in processors)
            {
                Node node = GetNode(pos, processor);
                if (node != null)
                {
                    success = false;
                    break;
                }
            }
            if (success)
                nodes.Add(pos);
        }
        return nodes;
    }

    public Processor GetProcessor(int index)
    {
        foreach (Processor obj in processors)
        {
            if (obj.index == index)
                return obj;
        }
        return null;
    }

    public Processor GetProcessor(string name)
    {
        foreach (Processor obj in processors)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }
    Node GetNode(Vector3Int position, Processor processor)
    {
        return processor.GetNode(position);
    }
    Node GetNode(Vector3Int position, string name)
    {
        Processor processor = GetProcessor(name);
        if (processor != null)
        {
            return processor.GetNode(position);
        }
        return null;
    }
    Node GetNode(Vector3Int position)
    {
        foreach (Processor processor in processors)
        {
            Node node = processor.GetNode(position);
            if (node != null)
                return node;
        }
        return null;
    }
    public int GetNodeCount(string name, bool requireActive = true)
    {
        Processor processor = GetProcessor(name);
        if (processor != null)
        {
            return processor.nodesActive.Count;
        }
        return 0;
    }
    public Node GetClosestNode(string name, Vector3 position)
    {
        Processor processor = GetProcessor(name);
        if (processor != null)
        {
            //Debug.Log("Get Closest Node: " + processor.name + ", " + processor.nodes.Count);
            float distanceMin = 10000f;
            Node node = null;
            foreach (Node nodeTemp in processor.nodes.Values)
            {
                if (nodeTemp.IsBusy())
                    continue;
                
                if (!nodeTemp.save.active)
                    continue;

                // check if node is directly accessible by neighbour positions
                List<Vector3Int> neighbours = GetOpenNeighbours(nodeTemp);
                if (neighbours.Count == 0)
                    continue;
                
                float distance = Vector3.Distance(position, nodeTemp.GetPosition());
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    node = nodeTemp;
                }
            }
            return node;
        }
        return null;
    }
    public List<NodeSave> GetNodeSaves()
    {
        List<NodeSave> nodes = new List<NodeSave>();
        foreach (Processor processor in processors)
        {
            foreach (Node node in processor.nodesList)
            {
                nodes.Add(node.save);
            }
        }
        return nodes;
    }
}


[Serializable]
public class Processor
{
    public string name;
    public int index;
    public GameObject prefab;
    public GameObject prefabTop;
    public int amount;
    public int[] clusterAmounts; // should we cluster these when we spawn them
    public bool refreshable;

    // runttime:
    public Dictionary<Vector3Int, Node> nodes;
    public List<Node> nodesList;
    public List<Node> nodesActive;

    public void Initialize()
    {
        nodes = new Dictionary<Vector3Int, Node>();
        nodesList = new List<Node>();
        nodesActive = new List<Node>();
    }
    public Node GetNode(Vector3Int position)
    {
        Node node;
        if (nodes.TryGetValue(position, out node))
            return node;
        
        return null;
    }
    public void AddNode(Node node)
    {
        nodes.Add(node.GetPosition(), node);

        nodesList.Add(node);
    }
    public void RemoveNode(Node node)
    {
        Node nodeTemp;
        if (nodes.TryGetValue(node.GetPosition(), out nodeTemp))
            nodes.Remove(node.GetPosition());

        if (nodesList.Contains(node))
            nodesList.Remove(node);
    }
    public void AddActiveNode(Node node)
    {
        if (nodesActive.Contains(node))
            return;
        
        nodesActive.Add(node);
    }
    public void RemoveActiveNode(Node node)
    {
        if (nodesActive.Contains(node))
            nodesActive.Remove(node);
    }
}

[Serializable]
public abstract class Node : MonoBehaviour
{
    public bool busy = false;
    protected Action<Node> OnDestroyFunc;
    protected Action<Node> OnActivateFunc;
    protected Func<Vector3Int, Node> GetNodeFunc;
    protected bool refreshable;
    public bool surface;
    public float planeHeight;

    [Header("Debug:")]
    public NodeSave save;

    public virtual void Initialize(Vector3Int position, Func<Vector3Int, Node> GetNodeFunc, Action<Node> OnDestroyFunc, Action<Node> OnActivateFunc, string nameProcessor, bool refreshable, float planeHeight)
    {
        this.OnDestroyFunc = OnDestroyFunc;
        this.OnActivateFunc = OnActivateFunc;
        this.GetNodeFunc = GetNodeFunc;
        this.refreshable = refreshable;
        this.planeHeight = planeHeight;

        save = new NodeSave(position, nameProcessor);

        busy = false;
        Activate(true);
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    public Vector3Int GetPosition()
    {
        return save.position;
    }
    public void SetBusy(bool val)
    {
        busy = val;
    }
    public bool IsBusy()
    {
        return busy;
    }
    public virtual void DestroyNode()
    {
        if (OnDestroyFunc!=null)
            OnDestroyFunc(this);
        
        Destroy(this.gameObject);
    }
    public void Activate(bool active)
    {
        this.gameObject.SetActive(active);
        save.active = active;

        OnActivateFunc(this);
    }
    public void InactiveCheck()
    {
        if (save.active)
            return;
        
        if (Time.fixedTime > save.time)
        {
            // respawn in
            Activate(true);
        }
    }
    public void SurfaceCheck()
    {
        if (Mathf.Abs(planeHeight - save.position.y) > 0.5f)
        {
            surface = false;
            return;
        }
        List<Vector3Int> dirs = new List<Vector3Int>() {Vector3Int.forward, -Vector3Int.forward, Vector3Int.right, -Vector3Int.right};
        List<Node> nodes = new List<Node>();
        foreach (Vector3Int dir in dirs)
        {
            Node node = GetNodeFunc(save.position + dir);
            if (node != null)
            {
                nodes.Add(node);
            }
        }
        if (nodes.Count < 4)
        {
            surface = true;
            return;
        }
        surface = false;
    }
}

[Serializable]
public class NodeSave
{
    public Vector3Int position;
    public bool active;
    public float time;
    public string nameProcessor;

    public NodeSave(Vector3Int position, string nameProcessor)
    {
        this.position = position;
        this.nameProcessor = nameProcessor;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColonistProcessor : MonoBehaviour
{
    /*
        Handles all spawning/despawning of in-game objects like food/minerals
    */
    [Header("Inputs:")]
    public List<Processor> processors;
    public MeshRenderer plane;
    public Bounds bounds;
    int areaIndex;

    public void Initialize(int areaIndex)
    {   
        this.areaIndex = areaIndex;
        bounds = plane.bounds;
        ProcessorSetup();
    }

    void ProcessorSetup()
    {
        foreach (Processor processor in processors)
        {
            processor.Initialize();
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
    }
    public void RefreshInactive()
    {
        foreach (Processor processor in processors)
        {
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
        Vector3Int key = new Vector3Int((int)pos.x, 0, (int)pos.z);
        obj.transform.position = key;
        Node node = obj.GetComponent<Node>();
        node.Initialize(areaIndex, key, NodeDestroyCallback, NodeActivateCallback, processor.name);
        processor.AddNode(node);
        if (processor.clusterAmount > 0)
        {
            // mineral case, where we wants clusters of minerals, not just randomly
            ClusterNode(node, processor.clusterAmount + UnityEngine.Random.Range(-8, 8), processor);
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
    void ClusterNode(Node nodeIn, int amount, Processor processor)
    {
        // this will spawn in other nodes of the same object around it randomly
        for (int i = 0; i < amount; i++)
        {
            List<Vector3Int> neighbours = GetOpenNeighbours(nodeIn);
            if (neighbours.Count > 0)
            {
                Vector3Int neighbourPosition = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
                GameObject newObj = Instantiate(nodeIn, this.transform).gameObject;
                newObj.transform.position = neighbourPosition;
                Node node = newObj.GetComponent<Node>();
                node.Initialize(areaIndex, neighbourPosition, NodeDestroyCallback, NodeActivateCallback, processor.name);
                nodeIn = node;

                processor.AddNode(node);
            } else
            {
                break;
            }
        }
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
        Processor processor = GetProcessor(node.nameProcessor);
        if (node.active)
        {
            processor.AddActiveNode(node);
        } else
        {
            processor.RemoveActiveNode(node);
        }
    }
    public Vector3 GetOpenPosition()
    {
        // try for 30 iterations
        for (int i = 0; i < 30; i++)
        {
            Vector3Int position = new Vector3Int(UnityEngine.Random.Range((int)bounds.min.x, (int)bounds.max.x), 0, UnityEngine.Random.Range((int)bounds.min.z, (int)bounds.max.z));
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
            Vector3 positionFixed = new Vector3(pos.x, -0.5f, pos.z);
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
                
                if (!nodeTemp.active)
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
}


[Serializable]
public class Processor
{
    public string name;
    public int index;
    public GameObject prefab;
    public int amount;
    public int clusterAmount = 0; // should we cluster these when we spawn them

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
    protected Vector3Int position;
    public bool busy = false;
    protected int areaIndex;
    protected Action<Node> OnDestroyFunc;
    protected Action<Node> OnActivateFunc;
    public string nameProcessor;

    [Header("Debug:")]
    public bool active = true;
    protected float time;

    public virtual void Initialize(int areaIndex, Vector3Int position, Action<Node> OnDestroyFunc, Action<Node> OnActivateFunc, string nameProcessor)
    {
        this.position = position;
        this.areaIndex = areaIndex;
        this.OnDestroyFunc = OnDestroyFunc;
        this.OnActivateFunc = OnActivateFunc;
        this.nameProcessor = nameProcessor;

        transform.position = position;
        busy = false;

        Activate(true);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public Vector3Int GetPosition()
    {
        return position;
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
        this.active = active;

        OnActivateFunc(this);
    }

    public void InactiveCheck()
    {
        if (active)
            return;
        
        if (Time.fixedTime > time)
        {
            // respawn in
            Activate(true);
        }
    }
}
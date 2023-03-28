using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabitationHandler : MonoBehaviour, IHandler
{
    public static HabitationHandler Instance;
    [Header("Inputs:")]
    public NodeProcessor _nodeProcessor;
    public HabBotProcessor _botProcessor;
    public HabitationParameters _parametersIn;
    public MeshRenderer _restBoundsDefault;
    public HabBotStateParameters _stateParameters;

    [Header("Debug:")]
    public Habitation _habitation;
    public List<HabitationQueue> _queues;
    public static HabitationParameters _parameters;

    public void Initialize()
    {
        Instance = this;
        _parameters = _parametersIn;
        _nodeProcessor.Initialize();
        _botProcessor.Initialize();
        
        _queues = new List<HabitationQueue>();
        _queues.Add(new HabitationQueue(HabBot.State.Craft){});
        _queues.Add(new HabitationQueue(HabBot.State.Machine){});
        _queues.Add(new HabitationQueue(HabBot.State.CollectFood){});
        _queues.Add(new HabitationQueue(HabBot.State.CollectMinerals){});
        _queues.Add(new HabitationQueue(HabBot.State.CollectTrees){});
        //_queues.Add(new HabitationQueue(){_state = HabBot.State.Heal, _nodes = new List<NodeObject>()});
    }
    public void Load(Habitation habitation = null, List<Node> nodes = null)
    {
        if (nodes != null)
        {
            _nodeProcessor.Load(nodes);
        } else
        {
            _nodeProcessor.Load(null);
        }
        if (habitation != null)
        {
            _habitation = habitation;
        } else
        {
            _habitation = new Habitation();
            _habitation.NewHabitation(_restBoundsDefault.bounds);
        }
         _habitation.Initialize(_stateParameters);
        Destroy(_restBoundsDefault.gameObject);
        UIHandler.Instance.InitializeHabitation(_habitation);
        SpawnHabitation();
    }
    void Update()
    {
        if (_habitation != null)
            _habitation.UpdateHabitation();
    }
    public void SpawnHabitation()
    {
        _botProcessor.Setup(_habitation, _nodeProcessor);
    }
    public void NewNode(Node node)
    {
        _nodeProcessor.AssignNode(node);
    }
    public NodeObject SpawnNodeUnassigned(Node node)
    {
        return _nodeProcessor.SpawNode(node);
    }
    public List<Node> GetAllNodes()
    {
        return _nodeProcessor.GetAllNodes();
    }
    public Habitation GetHabitation()
    {
        return _habitation;
    }
    public HabBot GetClosestBot(Vector3 position)
    {
        return _habitation.GetClosestBot(position);
    }
    public HabBotController GetBotController(HabBot bot)
    {
        return _botProcessor.GetController(bot);
    }
    public bool IsBotInjured()
    {
        return false;
    }
    public void AddInventory(int index)
    {
        _habitation.AddInventory(index);
    }
    public void RemoveInventory(int index)
    {
        _habitation.RemoveInventory(index);
    }
    public Item GetItem(int index)
    {
        return _habitation.GetItem(index);
    }
    public List<ItemInventory> GetItemInventoriesWithItem(ItemInput item)
    {
        return _habitation.GetItemInventoriesWithItem(item);
    }
    public void AddObjectToQueue(HabBot.State state, Queueable obj)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            obj._queued = true;
            queue.Add(obj);
        }
    }
    public void RemoveObjectFromQueue(HabBot.State state, Queueable obj)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            obj._queued = false;
            queue.Remove(obj);
        }
    }
    public Queueable GetQueuedObject(HabBot.State state)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            return queue.Pop();
        }
        return null;
    }
    HabitationQueue GetQueue(HabBot.State state)
    {
        foreach (HabitationQueue queue in _queues)
        {
            if (queue._state == state)
                return queue;
        }
        return null;
    }
    public bool IsAddonQueued(ItemInput item, HabBot bot)
    {
        // check to see if we have already queued this addon type for placement on this bot
        HabitationQueue queue = GetQueue(HabBot.State.Machine);
        foreach (Queueable queueable in queue._queueables)
        {
            MachineQueuable machine = queueable as MachineQueuable;
            if (machine._bot == bot)
            {
                if (machine._item._index == item._index)
                    return true;
            }
        }
        return false;
    }
    public NodeObject GetClosestNodeObjectOfType(Node.Type type, Vector3 position, int prefabIndex = 0)
    {
        return _nodeProcessor.GetClosestNodeObject(type, position, prefabIndex);
    }
    public NodeObject GetNodeObject(Node node)
    {
        return _nodeProcessor.GetNodeObject(node);
    }
    public NodeObject GetClosestNodeObjectWithItem(ItemInput item, Vector3 position)
    {
        List<ItemInventory> inventories = GetItemInventoriesWithItem(item);
        if (inventories != null && inventories.Count > 0)
        {
            List<NodeObject> nodes = new List<NodeObject>();
            foreach (ItemInventory inventory in inventories)
            {
                if (inventory._obj == null)
                    continue;
                
                NodeObject obj = inventory._obj.GetNodeObject();
                if (obj != null)
                {
                    nodes.Add(obj);
                }
            }
            float distanceMin = 10000f;
            NodeObject objectOut = null;
            foreach (NodeObject node in nodes)
            {
                float distance = Vector3.Distance(position, node.GetPosition());
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    objectOut = node;
                }
            }
            return objectOut;
        } else
        {
            return null;
        }
    }
}

[Serializable]
public class HabitationQueue
{
    // this class handles any queue's set by the player - for machining state mostly
    public HabBot.State _state;
    public List<Queueable> _queueables;

    public HabitationQueue(HabBot.State state)
    {
        _state = state;
        _queueables = new List<Queueable>();
    }
    public Queueable Pop()
    {
        if (_queueables.Count > 0)
        {
            Queueable obj = _queueables[0];
            _queueables.RemoveAt(0);
            return obj;
        }
        return null;
    }
    public void Add(Queueable obj)
    {
        _queueables.Add(obj);
    }
    public void Remove(Queueable obj)
    {
        if (_queueables.Contains(obj))
            _queueables.Remove(obj);
    }
}

[Serializable]
public class HabitationParameters
{
    [Tooltip("number of hab-bots to spawn in at start of game")]
    public int _botAmountStart = 3;
    [Tooltip("time to refresh resources in minutes")]
    public int _resourceRefreshTime = 5;
}
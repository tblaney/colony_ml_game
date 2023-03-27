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
    public List<HabitationQueue> _queues;
    public HabitationParameters _parametersIn;
    public MeshRenderer _restBoundsDefault;

    [Header("Debug:")]
    public Habitation _habitation;
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
         _habitation.Initialize();
        Destroy(_restBoundsDefault.gameObject);
        UIHandler.Instance.InitializeHabitation(_habitation);
        SpawnHabitation();
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
    public void AddObjectToQueue(HabBot.State state, QueueObject obj)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            queue.Add(obj);
        }
    }
    public void RemoveObjectFromQueue(HabBot.State state, QueueObject obj)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            queue.Remove(obj);
        }
    }
    public QueueObject GetQueuedObject(HabBot.State state)
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
    public NodeObject GetClosestNodeObjectOfType(Node.Type type, Vector3 position)
    {
        return _nodeProcessor.GetClosestNodeObject(type, position);
    }
}


[Serializable]
public class HabitationQueue
{
    // this class handles any queue's set by the player - for machining state mostly
    public HabBot.State _state;
    public List<QueueObject> _objs;
    public HabitationQueue(HabBot.State state)
    {
        _state = state;
        _objs = new List<QueueObject>();
    }
    public QueueObject Pop()
    {
        if (_objs.Count > 0)
        {
            QueueObject obj = _objs[0];
            _objs.RemoveAt(0);
            return obj;
        }
        return null;
    }
    public void Add(QueueObject obj)
    {
        _objs.Add(obj);
    }
    public void Remove(QueueObject obj)
    {
        if (_objs.Contains(obj))
            _objs.Remove(obj);
    }
}

public abstract class QueueObject
{

}

public class NodeQueueObject : QueueObject
{
    public NodeObject _nodeObject;
    public NodeQueueObject(NodeObject obj)
    {
        _nodeObject = obj;
    }
}

public class HealQueueObject : QueueObject
{
    public ItemInput _itemInput;
    public HabBot _bot;
    public HealQueueObject(ItemInput input, HabBot bot)
    {
        _itemInput = input;
        _bot = bot;
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
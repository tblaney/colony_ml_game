using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabitationHandler : MonoBehaviour, IHandler
{
    public static HabitationHandler Instance;
    [Header("Inputs:")]
    public HabitationProcessor _habitationProcessor;
    public NodeProcessor _nodeProcessor;
    List<HabitationQueue> _queues;

    public void Initialize()
    {
        Instance = this;
        _nodeProcessor.Initialize();
        _habitationProcessor.Initialize();
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
            _habitationProcessor.Load(habitation);
        } else
        {
            _habitationProcessor.Load(null);
        }
    }
    public void NewNode(Node node)
    {
        _nodeProcessor.AssignNode(node);
    }
    public Color GetBotColor(int index)
    {
        return _habitationProcessor.GetBotColor(index);
    }
    public List<Node> GetNodes()
    {
        return _nodeProcessor.GetNodes();
    }
    public Habitation GetHabitation()
    {
        return _habitationProcessor._habitation;
    }
    public Sprite GetStateSprite(HabBot.State state)
    {
        return _habitationProcessor.GetStateSprite(state);  
    }
    public bool IsBotInjured()
    {
        return false;
    }
    public void AddInventory(int index)
    {
        _habitationProcessor.AddInventory(index);
    }
    public void RemoveInventory(int index)
    {
        _habitationProcessor.RemoveInventory(index);
    }
    public void AddObjectToQueue(HabBot.State state, QueueObject obj)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            queue.Add(obj);
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
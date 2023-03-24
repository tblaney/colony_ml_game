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
        _queues.Add(new HabitationQueue(){_state = HabBot.State.Machining, _nodes = new List<NodeObject>()});
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
        _nodeProcessor.SpawnNode(node);
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
    public NodeObject GetQueuedObject(HabBot.State state)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            return queue.Pop();
        }
        return null;
    }
    public void AddObjectToQueue(NodeObject nodeObject, HabBot.State state)
    {
        HabitationQueue queue = GetQueue(state);
        if (queue != null)
        {
            queue.Add(nodeObject);
        }
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
}


[Serializable]
public class HabitationQueue
{
    // this class handles any queue's set by the player - for machining state mostly
    public HabBot.State _state;
    public List<NodeObject> _nodes;
    public NodeObject Pop()
    {
        if (_nodes.Count > 0)
        {
            NodeObject node = _nodes[0];
            _nodes.RemoveAt(0);
            return node;
        }
        return null;
    }
    public void Add(NodeObject node)
    {
        _nodes.Add(node);
    }
}

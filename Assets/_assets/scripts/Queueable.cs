using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Queueable 
{
    public enum Type
    {
        Node,
        Machine,
        Craft,
        Haul,
    }
    public Type _queueableType;
    public bool _queued;
}

[Serializable]
public class MachineQueuable : Queueable
{
    public ItemInput _item;
    public HabBot _bot;
    public MachineQueuable(ItemInput input, HabBot bot)
    {
        _item = input;
        _bot = bot;
        _queueableType = Type.Machine;
    }
}

[Serializable]
public class HaulQueueable : Queueable
{
    public ItemInput _item;
    public int _inventoryIn;
    public int _inventoryOut;
    public HaulQueueable(ItemInput item, int inventoryIn, int inventoryOut)
    {
        _item = item;
        _inventoryIn = inventoryIn;
        _inventoryOut = inventoryOut;
        _queueableType = Type.Haul;
    }
}

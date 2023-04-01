using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Queueable 
{
    [Header("Queueable Inputs:")]
    public HabBot.State _state;
    public bool _queued;

    public virtual void AddQueueable()
    {
        HabitationHandler.Instance.AddObjectToQueue(_state, this);
    }
    public virtual void DestroyQueueable()
    {
        HabitationHandler.Instance.RemoveObjectFromQueue(_state, this);
    }
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
        _state = HabBot.State.Machine;
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
        _state = HabBot.State.Haul;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateCraft : HabBotState
{
    public int _effectIndexIn;
    ItemInput _itemToCraft;
    NodeObject _targetNode;
    int _effectIndex;
    bool _buildStarted;

    List<NodeObject> _targetItemStorage;
    int _indexTargetItem;

    public enum Stage
    {
        ItemGather,
        Craft,
    }
    public Stage _stage;


    public override void StartState()
    {
        _stage = Stage.ItemGather;

        Queueable queueable = HabitationHandler.Instance.GetQueuedObject(HabBot.State.Craft);
        if (queueable == null)
        {
            _controller.SetState((int)HabBot.State.Idle);
            return;
        } else
        {
            _itemToCraft = queueable as ItemInput;
        }
    }
    void Refresh()
    {
        switch (_stage)
        {
            case Stage.ItemGather:
                List<ItemInput> items = InventoryCheck();
                if (items.Count > 0)
                {
                    // need to collect items before we move forward
                    
                } else
                {
                    _stage = Stage.Craft;
                    Refresh();
                }
                break;
            case Stage.Craft:
                break;
        }
    }
    List<ItemInput> InventoryCheck()
    {
        // check to see if hab bot has necessary items in inventory
        Item item = ItemHandler.Instance.GetUnlinkedItem(_itemToCraft._index);
        List<ItemInput> output = new List<ItemInput>();
        List<ItemInput> requirements = item._options._recipe;
        HabBot bot = _controller.GetBot();
        ItemInventory inventory = ItemHandler.Instance.GetItemInventory(bot._inventoryIndex);
        foreach (ItemInput itemInput in requirements)
        {
            if (!inventory.Contains(itemInput))
            {
                output.Add(itemInput);
            }
        }
        return output;
    }
    void BeginItemGrab()
    {

    }
    void RefreshCraftingTarget()
    {
        _targetNode = HabitationHandler.Instance.GetClosestNodeObjectWithOutputItem(_itemToCraft, transform.position);
        if (_targetNode == null)
        {
            _controller.SetState((int)HabBot.State.Idle);
            return;
        } else
        {
            _nav.MoveTo(_targetNode.GetPosition(), PathCallback);
        }
    }
    void RefreshItemRetrievalTarget()
    {

    }
    void PathCallback()
    {
        float distance = Vector3.Distance(transform.position, _targetNode.GetPosition());
        if (distance < _interactionDistance)
        {
            _controller.ActivateAddon(HabBotAddon.Type.Welder);
            if (_effectIndex == 0 && _effectIndexIn != 0)
                    _effectIndex = EffectHandler.Instance.SpawnEffect(_effectIndexIn, transform.position + transform.forward);
            
            _buildStarted = true;
            Invoke("BuildCompleteCallback", _controller.GetBot().GetInteractTime(HabBot.State.Craft));
        } else
        {
            // we popped the queue, so add it back in as we are not able to complete it
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Craft, _itemToCraft as Queueable);
            StartState();
        }
    }
    void BuildCompleteCallback()
    {
        /*
        BuiltNodeBCraft crafter = _targetNode as BuiltNodeObjectCraft;
        if (crafter != null)
        {
            crafter.FinishCraft();
            StartState();
            return;
        } else
        {
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Build, _targetNode as Queueable);
            StartState();
        }
        */
    }
    public override void StopState()
    {
        if (_effectIndex != 0)
            EffectHandler.Instance.StopEffect(_effectIndex);
        _controller.ClearAddons();
        _effectIndex = 0;
        base.StopState();
    }
    public override void UpdateState()
    {
        if (_targetNode == null)
            return;

        float distance = Vector3.Distance(transform.position, _targetNode.GetPosition());
        if (distance < 5f)
        {
            UpdateRotation((_targetNode.GetPosition() - transform.position).normalized);
        }
    }
}   

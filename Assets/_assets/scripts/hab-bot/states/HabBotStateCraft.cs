using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateCraft : HabBotState
{
    public int _effectIndexIn;
    ItemInput _itemToCraft;
    NodeObject _targetNode = null;
    List<NodeObject> _targetNodes;
    List<ItemInput> _itemsToRetrieve;
    int _effectIndex;
    bool _buildStarted;
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
        _targetNode = null;

        Queueable queueable = HabitationHandler.Instance.GetQueuedObject(HabBot.State.Craft);
        if (queueable == null)
        {
            _bot.StateFailure("No object to queue");
            return;
        } else
        {
            _itemToCraft = queueable as ItemInput;
        }
        InventoryCheck();
        RefreshTarget();
    }
    void RefreshTarget()
    {
        switch (_stage)
        {
            case Stage.ItemGather:
                if (_itemsToRetrieve.Count > 0 && _indexTargetItem < _itemsToRetrieve.Count)
                {
                    // need to collect items before we move forward
                    NodeObject node = _targetNodes[_indexTargetItem];
                    _nav.MoveTo(node.GetPosition(), RetrieveCallback);
                } else
                {
                    _stage = Stage.Craft;
                    RefreshTarget();
                }
                break;
            case Stage.Craft:
                _targetNode = HabitationHandler.Instance.GetClosestNodeObjectWithOutputItem(_itemToCraft, transform.position);
                if (_targetNode != null)
                {
                    _nav.MoveTo(_targetNode.GetPosition(), CraftCallback);
                } else
                {
                    _controller.GetBot().StateFailure("This bot can't carry the necessary items. Consider stockpiling.");
                    return;
                }
                break;
        }
    }
    void InventoryCheck()
    {
        _itemsToRetrieve = new List<ItemInput>();
        _targetNodes = new List<NodeObject>();
        _indexTargetItem = 0;
        // check to see if hab bot has necessary items in inventory
        Item item = ItemHandler.Instance.GetUnlinkedItem(_itemToCraft._index);
        List<ItemInput> output = new List<ItemInput>();
        List<ItemInput> requirements = item._options._recipe;
        HabBot bot = _controller.GetBot();
        ItemInventory inventory = ItemHandler.Instance.GetItemInventory(bot._inventoryIndex);
        if (!inventory.HasCapacity(requirements))
        {
            bot.StateFailure("This bot can't carry the necessary items. Consider stockpiling.");
            return;
        }
        foreach (ItemInput itemInput in requirements)
        {
            if (!inventory.Contains(itemInput))
            {
                NodeObject node = HabitationHandler.Instance.GetClosestNodeObjectWithItem(itemInput, transform.position);
                if (node == null)
                {
                    // cant fulfill request
                    bot.StateFailure("This bot couldn't locate the necessary items to build this item. You might be missing items.");
                    return;
                }
                _itemsToRetrieve.Add(itemInput);   
                _targetNodes.Add(node);
            }
        }
    }
    void RetrieveCallback()
    {
        // need to extract item from that inventory
        _targetNodes[_indexTargetItem].gameObject.GetComponent<InventoryObject>().RemoveItem(_itemsToRetrieve[_indexTargetItem]);
        ItemHandler.Instance.GetItemInventory(_controller.GetBot()._inventoryIndex).AddItem(_itemsToRetrieve[_indexTargetItem]);
        InventoryCheck();
        RefreshTarget();
    }
    void CraftCallback()
    {
        if (_targetNode == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, _targetNode.GetPosition());
        if (distance < _interactionDistance)
        {
            ItemInventory inventory = ItemHandler.Instance.GetItemInventory(_bot._inventoryIndex);
            Item item = ItemHandler.Instance.GetUnlinkedItem(_itemToCraft._index);
            List<ItemInput> requirements = item._options._recipe;
            foreach (ItemInput requirement in requirements)
            {
                inventory.RemoveItem(requirement);
            }
            inventory.AddItem(_itemToCraft);
            _bot.SetState(_bot._stateCache);
        } else
        {
            StartState();
        }
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

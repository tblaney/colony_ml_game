using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateMachine : HabBotState
{
    public int _effectIndexIn;
    MachineQueuable _queueable;
    int _effectIndex;
    NodeObject _targetNode;

    float _timer = 0f;

    public enum Stage
    {
        ItemGather,
        Heal,
    }
    public Stage _stage;


    public override void StartState()
    {
        _stage = Stage.ItemGather;
        _targetNode = null;
        _timer = 0f;

        Queueable queueable = HabitationHandler.Instance.GetQueuedObject(HabBot.State.Machine);
        if (queueable == null)
        {
            _bot.StateFailure("No object to queue");
            return;
        } else
        {
            _queueable = queueable as MachineQueuable;
        }
        InventoryCheck();
        RefreshTarget();
    }
    void RefreshTarget()
    {
        switch (_stage)
        {
            case Stage.ItemGather:
                if (InventoryCheck())
                {
                    _nav.MoveTo(_targetNode.GetPosition(), RetrieveCallback);
                }
                break;
            case Stage.Heal:

                break;
        }
    }
    bool InventoryCheck()
    {
        ItemInput input = _queueable._item;
        ItemInventory inventory = ItemHandler.Instance.GetItemInventory(_bot._inventoryIndex);
        if (!inventory.HasCapacity(new List<ItemInput>() {input}))
        {
            _bot.StateFailure("This bot can't carry the necessary items. Consider stockpiling.");
            return false;
        }
        if (inventory.Contains(input._index, input._amount))
        {
            _stage = Stage.Heal;
            RefreshTarget();
            return false;
        }
        NodeObject node = HabitationHandler.Instance.GetClosestNodeObjectWithItem(input, transform.position);
        if (node == null)
        {
            // cant fulfill request
            _bot.StateFailure("This bot couldn't locate the necessary items to build this item. You might be missing items.");
            return false;
        }
       _targetNode = node;
       return true;
    }
    void RetrieveCallback()
    {
        // need to extract item from that inventory
        _targetNode.gameObject.GetComponent<InventoryObject>().RemoveItem(_queueable._item);
        ItemHandler.Instance.GetItemInventory(_bot._inventoryIndex).AddItem(_queueable._item);
        _stage = Stage.Heal;
        RefreshTarget();
    }
    void HealCallback()
    {
        float distance = Vector3.Distance(transform.position, _queueable._bot.GetPosition());
        if (distance < _interactionDistance)
        {
            if (_effectIndex == 0 && _effectIndexIn != 0)
                    _effectIndex = EffectHandler.Instance.SpawnEffect(_effectIndexIn, transform.position + transform.forward);
            _bot.MachineApply(_queueable._item);
        } else
        {
            // we popped the queue, so add it back in as we are not able to complete it
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Machine, _queueable as Queueable);
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
        switch (_stage)
        {
            case Stage.ItemGather:
                if (_targetNode == null)
                    return;

                float distance = Vector3.Distance(transform.position, _targetNode.GetPosition());
                if (distance < 5f)
                {
                    UpdateRotation((_targetNode.GetPosition() - transform.position).normalized);
                }
                break;
            case Stage.Heal:
                _timer += Time.deltaTime;
                if (_timer > 2f)
                {
                    _nav.MoveTo(_queueable._bot.GetPosition(), HealCallback);
                }
                break;
        }
    }
}   

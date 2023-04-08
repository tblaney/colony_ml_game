using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateStockpile : HabBotState
{
    InventoryObject _targetInventory;
    public override void StartState()
    {
        Debug.Log("Hab Bot Stockpile Start State: ");

        _targetInventory = null;
        List<int> inventories = HabitationHandler.Instance.GetItemInventories(false);
        if (inventories.Count > 0)
        {
            // need to find closest one with available capacity
            List<ItemInventory> itemInventories = ItemHandler.Instance.GetItemInventories(inventories);
            float distanceMin = 10000f;
            foreach (ItemInventory inventory in itemInventories)
            {
                if (!inventory.CapacityCheck())
                    continue;
                InventoryObject obj = inventory._obj;
                if (obj == null)
                    continue;
                float distance = Vector3.Distance(transform.position, obj.GetPosition());
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    _targetInventory = obj;
                }
            }
        }
        if (_targetInventory != null)
        {
            _nav.MoveTo(_targetInventory.GetPosition(), PathCallback);
        } else
        {
            _bot.StateFailure("Unable to find a free chest.");
        }
    }
    void PathCallback()
    {
        if (_targetInventory == null)
        {
            StartState();
            return;
        }
        float distance = Vector3.Distance(transform.position, _targetInventory.GetPosition());
        Debug.Log("Hab Bot Stockpile Path Callback: " + distance);
        if (distance < _interactionDistance)
        {
            // drop off what we can
            ItemInventory inventory = ItemHandler.Instance.GetItemInventory(_bot._inventoryIndex);
            List<ItemInput> itemsToRemove = new List<ItemInput>();
            foreach (Item item in inventory._items)
            {
                ItemInput itemInput = item.GetItemInput();
                if (!inventory.HasCapacity(new List<ItemInput>(){itemInput}))
                    continue;
                _targetInventory.AddItem(itemInput);
                itemsToRemove.Add(itemInput);
            }
            foreach (ItemInput input in itemsToRemove)
            {
                inventory.RemoveItem(input);
            }
            _bot.SetState(_bot._stateCache);
        } else
        {
            StartState();
        }
    }
    public override void StopState()
    {
        base.StopState();
    }

    public override void UpdateState()
    {
        //_nav.SetVelocity(Vector3.zero);
        //throw new NotImplementedException();
    }
}   

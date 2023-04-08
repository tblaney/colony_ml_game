using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceNodeObject : NodeObject
{
    public List<ItemInputChance> _itemInputs;
    [SerializeField] private InteractableNode _interactable;

    public override void InitializeNode()
    {
        if (_interactable != null)
            _interactable.Setup(_node);
    }
    public override void OnDestroyNode()
    {
        // need to clone resource and add it to habitation inventory, or hab-bot inventory
        //HabitationHandler.Instance.GetHabitation().AddItem(_itemInput);
    }
    public List<ItemInput> GetItemInputs()
    {
        List<ItemInput> items = new List<ItemInput>();
        foreach (ItemInputChance itemInput in _itemInputs)
        {
            if (itemInput.IsHit())
            {
                itemInput._item._amount = UnityEngine.Random.Range(itemInput._amountRange.x, itemInput._amountRange.y);
                items.Add(itemInput._item);
            }
        }
        return items;
    }
}

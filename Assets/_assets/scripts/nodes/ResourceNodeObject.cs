using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceNodeObject : NodeObject
{
    public ItemInput _itemInput;
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
}

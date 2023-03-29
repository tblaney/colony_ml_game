using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableNode : Interactable
{
    Node _node;
    public Action<bool> OnHoverFunc;

    public void Setup(Node node)
    {
        _node = node;
    }
    public override void Interact()
    {
        UIHandler.Instance.ActivateNodeFocus(_node);
    }   
    public override void InteractHover(bool isIn)
    {
        PrefabInput prefabInput = PrefabHandler.Instance.GetPrefabInput(_node._prefab);
        UIHandler.Instance.ActivateTooltip(isIn, prefabInput._name);
        if (OnHoverFunc != null)
            OnHoverFunc(isIn);
        base.InteractHover(isIn);
    }
}

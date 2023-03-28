using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableBuiltObject : Interactable
{
    Building _building;
    Node _node;
    public Action<bool> OnHoverFunc;

    public void Setup(Node node, Building building)
    {
        _building = building;
        _node = node;
    }
    public override void Interact()
    {
        //throw new System.NotImplementedException();
        UIHandler.Instance.ActivateNodeFocus(_node);
    }

    public override void InteractHover(bool isIn)
    {
        UIHandler.Instance.ActivateTooltip(isIn, _building._name);
        if (OnHoverFunc != null)
            OnHoverFunc(isIn);
        base.InteractHover(isIn);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableBuiltObject : Interactable
{
    Building _building;
    public Action<bool> OnHoverFunc;

    public void Setup(Building building)
    {
        _building = building;
    }
    public override void Interact()
    {
        //throw new System.NotImplementedException();
    }

    public override void InteractHover(bool isIn)
    {
        UIHandler.Instance.ActivateTooltip(isIn, _building._name);
        if (OnHoverFunc != null)
            OnHoverFunc(isIn);
        base.InteractHover(isIn);
    }
}

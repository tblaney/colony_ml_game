using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableBuilding : Interactable
{
    Building _building;

    public void Setup(Building building)
    {
        _building = building;
    }
    public override void Interact()
    {
        
    }   
    public override void InteractHover(bool isIn)
    {
        UIHandler.Instance.ActivateTooltip(isIn, _building._name);
        base.InteractHover(isIn);
    }
}

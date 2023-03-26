using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHabBot : Interactable
{
    public override TooltipInfo GetTooltipInfo()
    {
        return base.GetTooltipInfo();
    }

    public override void Interact()
    {
        //throw new System.NotImplementedException();
    }

    public override void InteractHover(bool isIn)
    {
        base.InteractHover(isIn);
    }
}

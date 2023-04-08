using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHabBot : Interactable
{
    HabBot _bot;
    public void Setup(HabBot bot)
    {
        _bot = bot;
    }
    public override void Interact()
    {
        UIHandler.Instance.ActivateHabBotFocus(_bot);
    }

    public override void InteractHover(bool isIn)
    {
        if (_bot == null)
            return;
            
        UIHandler.Instance.ActivateTooltip(isIn, _bot._name);
        base.InteractHover(isIn);
    }
}

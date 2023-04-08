using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEnemy : Interactable
{
    IEnemy _enemy;

    public void Setup(IEnemy enemy)
    {
        _enemy = enemy;
    }
    public override void Interact()
    {
        UIHandler.Instance.ActivateEnemyFocus(_enemy);
    }

    public override void InteractHover(bool isIn)
    {
        if (_enemy == null)
            return;
            
        UIHandler.Instance.ActivateTooltip(isIn, _enemy.GetStrings()[0]);
        base.InteractHover(isIn);
    }
}

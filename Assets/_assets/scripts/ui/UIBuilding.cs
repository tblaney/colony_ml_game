using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBuilding : UIObject
{
    // sets necessary info for this built object, and displays options
    Building _building;
    public override void Initialize()
    {
        ActivateUI(false);
    }
    public void Setup(Building building)
    {
        _building = building;
        ActivateUI(true);
    }

    public override void DestroyUI()
    {
        base.DestroyUI();
    }
}

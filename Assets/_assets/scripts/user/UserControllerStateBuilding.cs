using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerStateBuilding : UserControllerState
{
    public Building _building;
    
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnStartState()
    {
        //_cam.SetManual();
        _building = BuildingHandler.Instance.GetBuilding(UIBuilding._currentBuildingIndex);
        Debug.Log("User Controller State Building");
    }

    public override void OnStopState()
    {
        base.OnStopState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }
}

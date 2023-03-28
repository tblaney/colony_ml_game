using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateRest : HabBotState
{
    Notification _notification;
    public override void StartState()
    {
        BuiltNodeObject obj = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Building, _controller.GetBot()._position, 6) as BuiltNodeObject;
        Vector3 position = (obj._behaviour as BuiltNodeBehaviourRestMachine).GetZonePosition();
        position.y = 30f;
        _nav.MoveTo(position, PathCallback);
    }
    void PathCallback()
    {
        _nav.Stop();
    }
    public override void StopState()
    {
        base.StopState();
    }
    public override void UpdateState()
    {

    }
}   

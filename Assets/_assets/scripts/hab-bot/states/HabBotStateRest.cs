using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateRest : HabBotState
{
    Notification _notification;
    float _timer;

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
        float distance = Vector3.Distance(transform.position, _nav.GetDestination());
        if (distance < 2f)
        {
            // heal
            _timer += Time.deltaTime;
            if (_timer > 1f)
            {
                _timer = 0f;
                _controller.GetBot().GetVitality("energy").Heal(5);
            }
        }
    }
}   

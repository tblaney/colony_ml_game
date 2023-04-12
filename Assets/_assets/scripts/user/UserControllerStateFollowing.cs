using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerStateFollowing : UserControllerState
{
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void OnStartState()
    {
        ITarget target = UserHandler._target;
        if (target != null)
        {
            _cam.SetTargetFollow(target);
            _cam.SetFOV(18f);
        } else
        {
            StopState();
        }
    }
    public override void OnStopState()
    {
        _cam.SetManualControl();
        base.OnStopState();
    }
    public override void UpdateState()
    {
        // lerp to corrent position

    }
}

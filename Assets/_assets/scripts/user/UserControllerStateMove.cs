using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerStateMove : UserControllerState
{
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void OnStartState()
    {
        _cam.SetMove(UserHandler._target.GetPosition());
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllerStateFollowing : UserControllerState
{
    HabBot _bot;
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void OnStartState()
    {
        _bot = UIHabBot._activeHabBotFollow;
        if (_bot != null)
        {
            _cam.SetTargetFollow(_bot);
            _cam.SetFOV(10f);
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

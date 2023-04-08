using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateIdle : HabBotState
{
    public override void StartState()
    {
        _animator.SetAnimationState("Grounded", 0.2f);

        _nav.SetVelocity(Vector3.zero);
        _nav.Stop();
    }

    public override void StopState()
    {
        base.StopState();
    }

    public override void UpdateState()
    {
        _nav.SetVelocity(Vector3.zero);
        //throw new NotImplementedException();
    }
}   

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateRoam : HabBotState
{
    [Header("Inputs:")]
    public float _roamRange = 20f;

    public override void StartState()
    {
        _nav.MoveToRandomLocation(_roamRange, RoamCallback);
    }
    void RoamCallback()
    {
        if (Utils.Tools.IsHit(0.1f))
        {
            // stop for period of time
            _nav.Stop();
            Invoke("RoamCallback", 10f);
        } else
        {
            _nav.MoveToRandomLocation(_roamRange, RoamCallback);
        }
    }
    public override void StopState()
    {
        base.StopState();
    }
    public override void UpdateState()
    {
        
    }
}   

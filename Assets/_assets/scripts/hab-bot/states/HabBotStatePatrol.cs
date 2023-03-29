using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStatePatrol : HabBotState
{
    HabBotAgent _agent;

    public override void Initialize()
    {
        _agent = GetComponent<HabBotAgent>();
    }
    public override void StartState()
    {
        _agent.Setup(_controller.GetBot());
    }

    public override void StopState()
    {
        base.StopState();
    }

    public override void UpdateState()
    {

    }
}

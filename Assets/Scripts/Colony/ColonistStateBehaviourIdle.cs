using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourIdle : ColonistStateBehaviour
{
    public override float CalculateDecisionReward()
    {
        return -1f;
    }

    public override float GetStateDistance()
    {
        return -1f;
    }

    public override void StartBehaviour()
    {
        //mover.StopMovement();
        //AddAgentReward(-1f);
        nav.Stop();
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {
        nav.Stop();
    }
}

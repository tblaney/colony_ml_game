using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EnemyStateBehaviourRoam : EnemyStateBehaviour
{
    public override void StartBehaviour()
    {
        nav.MoveToRandomLocation(agent.enemy.roamRange, RoamCallback);
    }
    void RoamCallback()
    {
        if (Utils.Tools.IsHit(0.1f))
        {
            // stop for period of time
            nav.Stop();
            Invoke("RoamCallback", 10f);
        } else
        {
            nav.MoveToRandomLocation(agent.enemy.roamRange, RoamCallback);
        }
    }
    public override void StopBehaviour()
    {
        nav.Stop();
    }
    public override bool RunCheck()
    {
        return true;
    }
    public override void UpdateBehaviour()
    {

    }

}
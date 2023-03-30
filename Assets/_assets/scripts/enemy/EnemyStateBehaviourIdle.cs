using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EnemyStateBehaviourIdle : EnemyStateBehaviour
{
    public override void StartBehaviour()
    {
        base.StartBehaviour();
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
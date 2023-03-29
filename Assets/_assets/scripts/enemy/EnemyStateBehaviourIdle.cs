using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EnemyStateBehaviourIdle : EnemyStateBehaviour
{
    public virtual void StartBehaviour()
    {

    }
    public virtual void StopBehaviour()
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
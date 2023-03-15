using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourIdle : ColonistStateBehaviour
{  
    public override void StartBehaviour()
    {
        //mover.StopMovement();
        AddAgentReward(-1f);
        nav.Stop();
    }
    public override void UpdateBehaviour()
    {
        nav.Stop();
    }
}

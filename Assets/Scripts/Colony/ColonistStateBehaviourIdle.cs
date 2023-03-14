using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourIdle : ColonistStateBehaviour
{  
    public override void StartBehaviour()
    {
        //mover.StopMovement();
        nav.Stop();
    }
    public override void UpdateBehaviour()
    {
        nav.Stop();
    }
}

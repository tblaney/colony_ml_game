using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourRest : ColonistStateBehaviour
{      
    Vector3 restPosition;
    bool hittingTarget;

    public override void StartBehaviour()
    {
        // setup the target position
        Vector3 restPosition = ColonyHandler.Instance.GetRestPosition(agent.areaIndex);
        //This should never happen
        if (restPosition == null) 
        {
            agent.SetState(0);
            return;
        }
        // Node node = restPosition as Node;
        // node.SetBusy(true);

        nav.MoveTo(restPosition, Rest);
        hittingTarget = false;
    }

    void Rest()
    {
        hittingTarget = true;
        nav.Stop();
        float distance = Vector3.Distance(transform.position, restPosition);
        //While in rest position heal over time
        agent.Energize((int)(agent.colonist.traits.laziness * 10));
        Invoke("Rest", 0.5f);
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {   

    }
}

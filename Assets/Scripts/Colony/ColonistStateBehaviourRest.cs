using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourRest : ColonistStateBehaviour
{      
    Vector3 restPosition;
    bool hittingTarget;
    bool healingDone;

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
        if (agent.colonist.energy < 0.5)
        {
            //AddAgentReward(1f);
        }
        // Node node = restPosition as Node;
        // node.SetBusy(true);

        nav.MoveTo(restPosition, Rest);
        hittingTarget = false;
        healingDone = false;
    }

    void Rest()
    {
        hittingTarget = true;
        nav.Stop();
        float distance = Vector3.Distance(transform.position, restPosition);
        //if (agent.colonist.energy < 1f)
        //AddAgentReward(0.1f);
        //else
        //if (healingDone)
        //    AddAgentReward(-0.25f);
        //While in rest position heal over time
        agent.Energize((int)(agent.colonist.traits.laziness * 10));
        if (agent.colonist.energy >= 1f && !healingDone)
        {
            agent.RequestDecision();
            healingDone = true;
            return;
        }
        Invoke("Rest", 0.5f);
    }

    public override void StopBehaviour()
    {
        if (agent.colonist.energy < 0.8f)
            AddAgentReward(-1f);
        
        healingDone = false;
        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {   

    }
    
    public override float GetStateDistance()
    {
        Vector3 restPosition = ColonyHandler.Instance.GetRestPosition(agent.areaIndex);

        return Vector3.Distance(transform.position, restPosition);
    }

    public override float CalculateDecisionReward()
    {
        if (agent.colonist.energy < 0.2f)
            return 1f;
        
        return -1f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourFood : ColonistStateBehaviour
{   
    public NodeObject targetNode;
    bool hittingTarget;

    public override void StartBehaviour()
    {
        if (targetNode != null)
        {
            targetNode.SetBusy(false);
        }
        // setup the target position
        targetNode = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Food, transform.position);
        if (targetNode == null)
        {
            agent.SetState(0);
            return;
        }
        targetNode.SetBusy(true);

        nav.MoveTo(targetNode.GetPosition(), CollectibleInteract);
        hittingTarget = false;
    }

    void CollectibleInteract()
    {
        if (targetNode == null)
        {
            StartBehaviour();
            return;
        }
        hittingTarget = true;
        nav.Stop();
        float distance = Vector3.Distance(transform.position, targetNode.GetPosition());
        if (distance > 2f)
        {
            StartBehaviour();
            return;
        }
        // interact with collectible, hit over time
        if (targetNode == null | targetNode.GetNode()._health._val <= 0)
        {
            // has been destroyed, search for another
            StartBehaviour();
            return;
        } 
        bool collectible_alive = targetNode.Damage((int)(agent.colonist.traits.nature * 10));
        if (!collectible_alive)
        {
            targetNode = null;
            agent.RequestDecision();
            ColonyHandler.Instance.AddFood(1);
            return;
        }
        Invoke("CollectibleInteract", 0.5f);
    }

    public override void StopBehaviour()
    {
        if (targetNode != null)
        {
            targetNode.SetBusy(false);
        }
        targetNode = null;
        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {
        if (targetNode == null)
        {
            StartBehaviour();
            return;
        }
        if (hittingTarget)
        {
            Vector3 targetPosition = targetNode.GetPosition();
            targetPosition.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*5f);
        }
    }

    public override float GetStateDistance()
    {
        if (targetNode != null)
        {
            return Vector3.Distance(transform.position, targetNode.GetPosition());
        }
        NodeObject nodeTemp = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Food, transform.position);
        if (nodeTemp != null)
            return Vector3.Distance(transform.position, nodeTemp.GetPosition());
        
        return -1f;
    }
    public override float CalculateDecisionReward()
    {
        int countColonist = ColonyHandler.Instance.GetColonistAmount();
        if (agent.colonist.energy > 0.2f && countColonist < 20)
            return 1f;
        
        return -1f;
    }
}

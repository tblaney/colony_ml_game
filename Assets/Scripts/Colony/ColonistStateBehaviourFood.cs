using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourFood : ColonistStateBehaviour
{   
    Collectible collectible;
    bool hittingTarget;

    public override void StartBehaviour()
    {
        Node node = null;
        if (collectible != null)
        {
            node = collectible as Node;
            node.SetBusy(false);
        }
        // setup the target position
        collectible = ColonyHandler.Instance.GetClosestCollectible(Collectible.Type.Food, agent.areaIndex, transform.position);
        if (collectible == null)
        {
            agent.SetState(0);
            return;
        }
        node = collectible as Node;
        node.SetBusy(true);

        nav.MoveTo(collectible.GetPosition(), CollectibleInteract);
        hittingTarget = false;
    }

    void CollectibleInteract()
    {
        if (collectible == null)
        {
            StartBehaviour();
            return;
        }
        hittingTarget = true;
        nav.Stop();
        float distance = Vector3.Distance(transform.position, collectible.GetPosition());
        if (distance > 2f)
        {
            StartBehaviour();
            return;
        }
        // interact with collectible, hit over time
        if (collectible.health <= 0)
        {
            // has been destroyed, search for another
            StartBehaviour();
            return;
        } 
        bool collectible_alive = collectible.Damage((int)(agent.colonist.traits.nature * 10));
        if (!collectible_alive)
        {
            // assign a reward for destroying something
            //AddAgentReward(1f);
            collectible = null;
            agent.RequestDecision();
            return;
        }
        Invoke("CollectibleInteract", 0.5f);
    }

    public override void StopBehaviour()
    {
        if (collectible != null)
        {
            Node node = collectible as Node;
            node.SetBusy(false);
        }
        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {
        if (collectible == null)
        {
            StartBehaviour();
            return;
        }
        if (hittingTarget)
        {
            Vector3 targetPosition = collectible.GetPosition();
            targetPosition.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*5f);
        }
    }

    public override float GetStateDistance()
    {
        Collectible collectibleTemp = ColonyHandler.Instance.GetClosestCollectible(Collectible.Type.Food, agent.areaIndex, transform.position);
        if (collectibleTemp != null)
            return Vector3.Distance(transform.position, collectibleTemp.GetPosition());
        
        return -1f;
    }

    public override float CalculateDecisionReward()
    {
        int countFood = ColonyHandler.Instance.GetCollectibleCount(Collectible.Type.Food, agent.areaIndex);
        //int enemyCount = ColonyHandler.Instance.GetEnemyAmount(agent.areaIndex);
        if (countFood > 1 && agent.colonist.energy > 0.2f)
            return 1f;
        
        return -1f;
    }
}

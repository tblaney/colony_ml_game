using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistStateBehaviourFood : ColonistStateBehaviour
{   
    Collectible collectible;
    bool hittingTarget;

    public override void StartBehaviour()
    {
        // setup the target position
        collectible = ColonyHandler.Instance.GetClosestCollectible(Collectible.Type.Food, agent.areaIndex, transform.position);
        if (collectible == null)
        {
            agent.SetState(0);
            return;
        }
        Node node = collectible as Node;
        node.SetBusy(true);

        nav.MoveTo(collectible.GetPosition(), CollectibleInteract);
        hittingTarget = false;
    }

    void CollectibleInteract()
    {
        hittingTarget = true;
        nav.Stop();
        float distance = Vector3.Distance(transform.position, collectible.GetPosition());
        // interact with collectible, hit over time
        if (collectible == null | collectible.health <= 0)
        {
            // has been destroyed, search for another
            StartBehaviour();
            return;
        } 
        collectible.Damage((int)(agent.colonist.traits.nature * 10));
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
        if (hittingTarget)
        {
            Vector3 targetPosition = collectible.GetPosition();
            targetPosition.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*5f);
        }
    }
}

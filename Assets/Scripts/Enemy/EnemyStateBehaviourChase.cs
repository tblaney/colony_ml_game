using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EnemyStateBehaviourChase : EnemyStateBehaviour
{
    public bool active = false;
    ColonistAgent targetAgent;
    bool cooldown = false;
    Vector3 positionCache;
    public override void StartBehaviour()
    {
        Refresh();
    }
    void Refresh()
    {
        ColonistAgent colonist = ColonyHandler.Instance.GetClosestColonist(areaIndex, transform.position);
        float distance = Vector3.Distance(transform.position, colonist.GetPosition());
        if (distance <= agent.chaseRadius){
            active = true;
            if (colonist == null)
            {
                active = false;
                nav.Stop();
                Invoke("CooldownCallback", 1f);
                return;
            }
            targetAgent = colonist;
            positionCache = colonist.GetPosition();
            nav.MoveTo(positionCache, NavCallback);
        } else {
            active = true;
            RoamRefresh();
        }
        
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
        if (!active)
            return;
        
        if (targetAgent == null || targetAgent.gameObject == null)
        {
            Refresh();
            return;
        }

        if (cooldown)
        {
            UpdateRotation((targetAgent.GetPosition() - transform.position).normalized);
            return;
        }

        float distance = Vector3.Distance(positionCache, targetAgent.GetPosition());
        if (distance > 2f)
        {
            // repath
            positionCache = targetAgent.GetPosition();
            nav.MoveTo(positionCache, NavCallback);
        }
    }
    void AttackTarget()
    {
        nav.Stop();
        targetAgent.Damage(agent.enemy.damage);
        cooldown = true;
        Invoke("CooldownCallback", 1f);
    }
    void CooldownCallback()
    {
        cooldown = false;
        NavCallback();
    }
    void NavCallback()
    {
        if (targetAgent == null || targetAgent.gameObject == null)
        {
            Refresh();
            return;
        }
        float distance = Vector3.Distance(transform.position, targetAgent.GetPosition());
        if (distance < 2f)
        {
            AttackTarget();
        } else
        {
            Refresh();
        }
    }
    void RoamRefresh()
    {
        nav.MoveToRandomLocation(16f, NavCallbackRoam);
    }
    void NavCallbackRoam()
    {
        Refresh();
    }
}

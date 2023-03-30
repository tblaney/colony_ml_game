using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ColonistStateBehaviourHeal : ColonistStateBehaviour
{   
    float timer = 0f;
    bool chasing = false;
    bool cooldown = false;
    ColonistAgent targetAgent;

    Vector3 positionCache;
    
    public override void StartBehaviour()
    {
        // agent patrols in a random pattern, and checks all agents for health status
        timer = 0f;
        cooldown = false;

        RoamRefresh();
    }

    public override void StopBehaviour()
    {

        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {        
        if (chasing)
        {
            if (targetAgent == null || targetAgent.gameObject == null)
            {
                StartBehaviour();
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
                nav.MoveTo(positionCache, ChaseRefresh);
            } else {
                HealTarget();
            }
            return;
        }
        
        timer += Time.fixedDeltaTime;
        if (timer > 2f)
        {
            timer = 0f;
            ChaseCheck();
            if (chasing)
            {
                nav.Stop();
                CancelInvoke();
                ChaseRefresh();
            }
        }
    }

    void RoamRefresh()
    {
        nav.MoveToRandomLocation(16f, NavCallbackRoam);
    }

    void ChaseRefresh()
    {
        if (targetAgent == null)
        {
            StartBehaviour();
            return;
        }
        positionCache = targetAgent.GetPosition();
        nav.MoveTo(positionCache, ChaseRefresh);
    }

    void ChaseCheck()
    {
        ColonistAgent closestAgent = ColonyHandler.Instance.GetClosestInjuredColonist(transform.position);
        if (closestAgent != null)
        {
            float distance = Vector3.Distance(transform.position, closestAgent.GetPosition());
            //Can add chase radius if we want but we discussed just having healers move to the nearest injured agent
            targetAgent = closestAgent;
            chasing = true;
        }
    }

    void NavCallbackRoam()
    {
        RoamRefresh();
        /*
        bool isHit = Tools.IsHit(0.3f);
        // 50% chance to just idle for x amount of time
        if (isHit)
        {
            nav.Stop();
            Invoke("NavCallbackRoam", UnityEngine.Random.Range(5f, 15f));
        } else
        {
            // new path to random location
            RoamRefresh();
        }
        */
    }

    void NavCallbackChase()
    {
        // try and attack target
        if (targetAgent == null || targetAgent.gameObject == null)
        {
            StartBehaviour();
            return;
        }
        float distance = Vector3.Distance(transform.position, targetAgent.GetPosition());
        if (distance < 2f)
        {
            //AddAgentReward(1f);
            HealTarget();
        } else
        {
            //agent.RequestDecision();
            ChaseRefresh();
        }
    }

    void HealTarget()
    {
        if (targetAgent == null)
        {
            agent.RequestDecision();
            //StartBehaviour();
            return;
        }
        // nav.Stop();
        if (targetAgent.IsInjured()){
            //Negative damage heals the target
            targetAgent.Damage((int)(-1.0*agent.colonist.traits.healing*10f));
            cooldown = true;
            Invoke("CooldownCallback", 1f);
        } else {
            chasing = false;
        }
        //agent.RequestDecision();
    }

    void CooldownCallback()
    {
        cooldown = false;
        HealTarget();
    }

    public override float GetStateDistance()
    {
        ColonistAgent closestAgent = ColonyHandler.Instance.GetClosestInjuredColonist(transform.position);
        if (closestAgent != null)
        {
            return Vector3.Distance(transform.position, closestAgent.GetPosition());
        }
        return -1f;
    }

    public override float CalculateDecisionReward()
    {
        ColonistAgent closestAgent = ColonyHandler.Instance.GetClosestInjuredColonist(transform.position);
        if (closestAgent != null)
        {
            return 1f;
        }
        return -1f;
    }
}

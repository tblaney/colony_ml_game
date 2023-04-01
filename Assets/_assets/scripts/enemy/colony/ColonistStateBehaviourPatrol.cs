using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ColonistStateBehaviourPatrol : ColonistStateBehaviour
{   
    public float chaseRadius = 20f;
    bool chasing = false;
    bool cooldown = false;
    float timer = 0f;
    HabBot targetBot;

    Vector3 positionCache;
    bool roaming = false;
    
    public override void StartBehaviour()
    {
        // agent patrols in a random pattern, and checks for nearest enemy entity for chase mechanics
        chasing = false;
        targetBot = null;
        timer = 0f;
        cooldown = false;
        roaming = false;

        if (!ChaseCheck())
            return;
        if (!chasing)
            RoamRefresh();
        else
            ChaseRefresh();
    }

    public override void StopBehaviour()
    {
        if (targetBot != null && targetBot.GetHealth() > 1)
        {
            AddAgentReward(-1f);
        }
        base.StopBehaviour();
    }

    public override void UpdateBehaviour()
    {        
        if (chasing)
        {
            if (targetBot == null)
            {
                //agent.RequestDecision();
                StartBehaviour();
                return;
            }

            if (cooldown)
            {
                UpdateRotation((targetBot.GetPosition() - transform.position).normalized);
                return;
            }

            float distance = Vector3.Distance(positionCache, targetBot.GetPosition());
            if (distance > 2f)
            {
                // repath
                positionCache = targetBot.GetPosition();
                nav.MoveTo(positionCache, NavCallbackChase);
            }
            return;
        }
        
        timer += Time.fixedDeltaTime;
        if (timer > 2f)
        {
            timer = 0f;
            if (!ChaseCheck())
                return;
            if (chasing)
            {
                nav.Stop();
                CancelInvoke();
                ChaseRefresh();
            } else
            {
                agent.RequestDecision();
            }
        }
    }

    void RoamRefresh()
    {
        nav.MoveToRandomLocation(16f, NavCallbackRoam);
    }

    void ChaseRefresh()
    {
        positionCache = targetBot.GetPosition();
        nav.MoveTo(positionCache, NavCallbackChase);
    }

    bool ChaseCheck()
    {
        HabBot enemy = HabitationHandler.Instance.GetClosestBot(transform.position);
        if (enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distance < chaseRadius)
            {
                targetBot = enemy;
                chasing = true;
            } else
            {
                if (Utils.Tools.IsHit(0.4f))
                {
                    if (Utils.Tools.IsHit(0.5f))
                    {
                        agent.SetState(2);
                    } else
                    {
                        agent.SetState(5);
                    }
                    return false;
                }
            }
        }
        return true;
    }

    void NavCallbackRoam()
    {
        RoamRefresh();
    }

    void NavCallbackChase()
    {
        // try and attack target
        if (targetBot == null)
        {
            agent.RequestDecision();
            //StartBehaviour();
            return;
        }
        float distance = Vector3.Distance(transform.position, targetBot.GetPosition());
        if (distance < 2f)
        {
            AttackTarget();
        } else
        {
            //agent.RequestDecision();
            ChaseRefresh();
        }
        agent.RequestDecision();
    }

    void AttackTarget()
    {
        nav.Stop();
        bool enemy_alive = targetBot.Damage((int)(agent.colonist.traits.attackStrength*20f));
        if (!enemy_alive)
        {
            AddAgentReward(1f);
        }
        cooldown = true;
        Invoke("CooldownCallback", 1f);
        agent.RequestDecision();
    }

    void CooldownCallback()
    {
        cooldown = false;
        NavCallbackChase();
    }

    public override float GetStateDistance()
    {
        HabBot enemy = HabitationHandler.Instance.GetClosestBot(transform.position);
        if (enemy != null)
        {
            return Vector3.Distance(transform.position, enemy.GetPosition());
        }
        return -1f;
    }

    public override float CalculateDecisionReward()
    {
        HabBot enemy = HabitationHandler.Instance.GetClosestBot(transform.position);
        if (enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distance < chaseRadius)
                return 1f;
        }
        return -1f;
    }
}

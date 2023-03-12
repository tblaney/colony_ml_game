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
    EnemyAgent targetAgent;

    Vector3 positionCache;
    
    public override void StartBehaviour()
    {
        // agent patrols in a random pattern, and checks for nearest enemy entity for chase mechanics
        chasing = false;
        targetAgent = null;
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
                nav.MoveTo(positionCache, NavCallbackChase);
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
        positionCache = targetAgent.GetPosition();
        nav.MoveTo(positionCache, NavCallbackChase);
    }

    void ChaseCheck()
    {
        EnemyAgent enemy = ColonyHandler.Instance.GetClosestEnemy(agent.areaIndex, transform.position);
        if (enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.GetPosition());
            if (distance < chaseRadius)
            {
                targetAgent = enemy;
                chasing = true;
            }
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
            AttackTarget();
        } else
        {
            ChaseRefresh();
        }
    }

    void AttackTarget()
    {
        nav.Stop();
        targetAgent.Damage((int)(agent.colonist.traits.attackStrength*20f));
        cooldown = true;
        Invoke("CooldownCallback", 1f);
    }

    void CooldownCallback()
    {
        cooldown = false;
        NavCallbackChase();
    }
}

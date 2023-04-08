using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EnemyStateBehaviourChase : EnemyStateBehaviour
{
    HabBot target;
    bool cooldown = false;
    Vector3 positionCache;
    public override void StartBehaviour()
    {
        Refresh();
    }
    void Refresh()
    {
        HabBot bot = HabitationHandler.Instance.GetClosestBot(transform.position);
        if (bot == null)
        {
            nav.Stop();
            Invoke("CooldownCallback", agent.enemy.cooldown);
            return;
        } else
        {
            target = bot;
            positionCache = target.GetPosition();
            nav.MoveTo(positionCache, NavCallback);
        }
    }
    public override void StopBehaviour()
    {
        nav.Stop();
    }
    public override bool RunCheck()
    {
        HabBot bot = HabitationHandler.Instance.GetClosestBot(transform.position);
        float distance = Vector3.Distance(transform.position, bot.GetPosition());
        if (distance < agent.enemy.chaseRadius)
        {
            return true;
        }
        return false;
    }
    public override void UpdateBehaviour()
    {
        if (target == null)
        {
            Refresh();
            return;
        }
        float distanceToTarget = Vector3.Distance(transform.position, target.GetPosition());
        if (distanceToTarget > agent.enemy.chaseRadius)
        {
            agent.SetDefaultState();
            return;
        }
        if (cooldown)
        {
            UpdateRotation((target.GetPosition() - transform.position).normalized);
            nav.Stop();
            return;
        }
        float distance = Vector3.Distance(positionCache, target.GetPosition());
        if (distance > agent.enemy.hitRadius)
        {
            // repath
            positionCache = target.GetPosition();
            nav.MoveTo(positionCache, NavCallback);
        }
    }
    void AttackTarget()
    {
        nav.Stop();
        animator.SetAnimationState("Attack", 0.1f);
        target.Damage((int)agent.enemy.damage);
        cooldown = true;
        Invoke("CooldownCallback", agent.enemy.cooldown);
    }
    void CooldownCallback()
    {
        cooldown = false;
        NavCallback();
    }
    void NavCallback()
    {
        if (target == null)
        {
            Refresh();
            return;
        }
        float distance = Vector3.Distance(transform.position, target.GetPosition());
        if (distance < agent.enemy.hitRadius)
        {
            AttackTarget();
        } else
        {
            Refresh();
        }
    }
}
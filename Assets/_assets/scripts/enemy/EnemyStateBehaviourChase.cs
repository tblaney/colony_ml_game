using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class EnemyStateBehaviourChase : EnemyStateBehaviour
{
    public bool active = false;
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
        float distance = Vector3.Distance(transform.position, bot.GetPosition());
        if (distance < agent.enemy.chaseRadius){
            active = true;
            if (bot == null)
            {
                active = false;
                nav.Stop();
                Invoke("CooldownCallback", agent.enemy.cooldown);
                return;
            }
            target = bot;
            positionCache = target.GetPosition();
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
        {
            return;
        }
        if (target == null)
        {
            Refresh();
            return;
        }
        if (cooldown)
        {
            UpdateRotation((target.GetPosition() - transform.position).normalized);
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
    void RoamRefresh()
    {
        nav.MoveToRandomLocation(agent.enemy.roamRange, NavCallbackRoam);
    }
    void NavCallbackRoam()
    {
        Refresh();
    }
}
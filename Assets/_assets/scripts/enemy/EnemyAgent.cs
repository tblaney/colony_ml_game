using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAgent : MonoBehaviour
{
    [Header("Inputs:")]
    public List<EnemyStateBehaviour> states;
    public Action<EnemyAgent> OnDestroyFunc;
    public bool randomize;

    [Space(20)]
    [Header("Stats:")]
    public float healthBase;
    public float healthDev;
    public float damageBase;
    public float damageDev;
    public float speedBase;
    public float speedDev;
    public float chaseRadiusBase;
    public float hitRadiusBase = 2f;
    public float attackCooldown = 1f;
    public float roamRangeBase = 16f;

    [Space(20)]
    [Header("Debug:")]
    public Enemy enemy;

    public EnemyStateBehaviour currentState;
    protected NavigationController nav;

    void Awake() {
        //Need to initialize enemy stats on awake to properly set enemy behaviour
        if (randomize)
        {
            enemy.InitializeRandom(this);
        }
        else
        {
            enemy.Initialize(this);
        }
        nav = GetComponent<NavigationController>();
        nav.SetSpeed(enemy.speed);
    }

    public void Setup(Action<EnemyAgent> destroyFunc)
    {
        this.OnDestroyFunc = destroyFunc;
        foreach (EnemyStateBehaviour stateBehaviour in states)
        {
            stateBehaviour.Initialize(nav);
        }
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    void Update()
    {
        StateUpdate();
    }
    void StateUpdate()
    {
        if (currentState == null)
        {
            currentState = GetState(0);
            currentState.StartBehaviour();
            return;
        }

        foreach (EnemyStateBehaviour state in states)
        {
            if (currentState != null && currentState == state)
                continue;
            
            if (state.priority > currentState.priority && state.RunCheck())
            {
                if (currentState != null)
                    currentState.StopBehaviour();
                
                currentState = state;
                currentState.StartBehaviour();
            }
        }

        if (currentState != null)
            currentState.UpdateBehaviour();
    }

    public bool Damage(int val)
    {
        // will return true if enemy is still alive
        enemy.health -= val;
        Debug.Log("Enemy took " + val.ToString() + " Damage and is at " + enemy.health.ToString() + " health");
        if (enemy.health <= 0)
        {
            Die();
            return false;
        }
        return true;
    }

    public EnemyStateBehaviour GetState(int priority)
    {
        foreach (EnemyStateBehaviour state in states)
        {   
            if (state.priority == priority)
                return state;
        }
        return null;
    }

    void Die()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc(this);
        
        Destroy(this.gameObject);
    }

    public void DestroyAgent()
    {
        Destroy(this.gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}

[Serializable]
public class Enemy
{
    public float health;
    public float damage;
    public float speed;
    public float chaseRadius;
    public float hitRadius;
    public float cooldown;
    public float roamRange;

    public void InitializeRandom(EnemyAgent agent)
    {
        health = agent.healthBase + UnityEngine.Random.Range(-agent.healthDev, agent.healthDev);
        damage = agent.damageBase + UnityEngine.Random.Range(-agent.damageDev, agent.damageDev);
        speed = agent.speedBase + UnityEngine.Random.Range(-agent.speedDev, agent.speedDev);
        chaseRadius = agent.chaseRadiusBase;
        hitRadius = agent.hitRadiusBase;
        cooldown = agent.attackCooldown;
        roamRange = agent.roamRangeBase;
    }

    public void Initialize(EnemyAgent agent)
    {
        health = agent.healthBase;
        damage = agent.damageBase;
        speed = agent.speedBase;
        chaseRadius = agent.chaseRadiusBase;
        hitRadius = agent.hitRadiusBase;
        cooldown = agent.attackCooldown;
        roamRange = agent.roamRangeBase;
    }
}

public interface IDamageable
{
    void Damage(int val);
}
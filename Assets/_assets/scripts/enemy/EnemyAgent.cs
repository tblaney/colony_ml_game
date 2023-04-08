 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAgent : MonoBehaviour, IEnemy, ITarget
{
    [Header("Inputs:")]
    public List<EnemyStateBehaviour> states;
    public Action<EnemyAgent> OnDestroyFunc;
    public bool randomize;

    [Space(20)]
    [Header("Stats:")]
    public int healthBase;
    public int healthDev;
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

    AnimatorHandler animator;

    public EnemyStateBehaviour currentState;
    protected NavigationController nav;

    void Awake() 
    {        
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
        animator = GetComponent<AnimatorHandler>();
    }
    void Start()
    {
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
            SetDefaultState();
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
        enemy.health.Damage(val);
        Debug.Log("Enemy took " + val.ToString() + " Damage and is at " + enemy.health.ToString() + " health");
        if (enemy.health._val <= 0)
        {
            Die();
            return false;
        }
        return true;
    }
    public void SetDefaultState()
    {
        if (currentState != null)
            currentState.StopBehaviour();
            
        currentState = GetState(0);
        currentState.StartBehaviour();
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
    
    public Vitality GetVitality()
    {
        return enemy.health;
    }
    public string[] GetStrings()
    {
        return new string[] {enemy.name, enemy.description};
    }
    public ITarget GetTarget()
    {
        return this;
    }
}

[Serializable]
public class Enemy
{
    public Vitality health;
    public string name;
    public string description;
    public float damage;
    public float speed;
    public float chaseRadius;
    public float hitRadius;
    public float cooldown;
    public float roamRange;

    public void InitializeRandom(EnemyAgent agent)
    {
        health = new Vitality() {_name = "health", _val = agent.healthBase + UnityEngine.Random.Range(-agent.healthDev, agent.healthDev)};
        damage = agent.damageBase + UnityEngine.Random.Range(-agent.damageDev, agent.damageDev);
        speed = agent.speedBase + UnityEngine.Random.Range(-agent.speedDev, agent.speedDev);
        chaseRadius = agent.chaseRadiusBase;
        hitRadius = agent.hitRadiusBase;
        cooldown = agent.attackCooldown;
        roamRange = agent.roamRangeBase;
    }

    public void Initialize(EnemyAgent agent)
    {
        health = new Vitality() {_name = "health", _val = agent.healthBase};
        damage = agent.damageBase;
        speed = agent.speedBase;
        chaseRadius = agent.chaseRadiusBase;
        hitRadius = agent.hitRadiusBase;
        cooldown = agent.attackCooldown;
        roamRange = agent.roamRangeBase;
    }
}

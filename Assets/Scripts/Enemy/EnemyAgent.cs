using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyAgent : MonoBehaviour, IDamageable
{
    [Header("Inputs:")]
    int areaIndex;
    public List<EnemyStateBehaviour> states;
    public Action<EnemyAgent> OnDestroyFunc;

    [Space(20)]
    [Header("Debug:")]
    public Enemy enemy;


    public EnemyStateBehaviour currentState;

    public void Setup(Enemy enemy, int areaIndex, Action<EnemyAgent> destroyFunc)
    {
        this.enemy = enemy;
        this.areaIndex = areaIndex;
        this.OnDestroyFunc = destroyFunc;

        foreach (EnemyStateBehaviour stateBehaviour in states)
        {
            stateBehaviour.Initialize(areaIndex);
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

    public void Damage(int val)
    {
        Debug.Log("Enemy Agent Damange: " + val);
        enemy.health -= val;
        if (enemy.health <= 0)
        {
            Die();
        }
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
    public int health;
    public int damage;

    public void InitializeRandom()
    {
        health = 100 + UnityEngine.Random.Range(-30, 30);
        damage = 10 + UnityEngine.Random.Range(0, 10);
    }
}

public interface IDamageable
{
    void Damage(int val);
}


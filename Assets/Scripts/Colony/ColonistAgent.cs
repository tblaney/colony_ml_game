using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;
using Utils;

public class ColonistAgent : Agent, IDamageable
{
    [Header("Inputs:")]
    public bool heuristics;
    List<ColonistStateBehaviour> stateBehaviours;
    public int areaIndex = 0;

    [Header("Debug:")]
    public Colonist colonist;
    public ColonistStateBehaviour currentBehaviour;
    public float timer;
    Action<ColonistAgent> OnDestroyFunc;

    //--------------------------------------------------------------

    public override void Initialize()
    {

    }

    public void Setup(Colonist colonist, int index, Action<ColonistAgent> OnDestroyFunc)
    {
        this.colonist = colonist;
        areaIndex = index;
        this.OnDestroyFunc = OnDestroyFunc;

        ColonistStateBehaviour[] behaviours = GetComponents<ColonistStateBehaviour>();
        stateBehaviours = new List<ColonistStateBehaviour>();
        foreach (ColonistStateBehaviour behaviour in behaviours)
        {
            stateBehaviours.Add(behaviour);
        }

        if (heuristics)
        {
            if (Tools.IsHit(0.5f))
            {
                SetState((int)Colonist.State.Patrol);
            } else
            {
                SetState((int)Colonist.State.Collect);
            }
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;
        
        if (currentBehaviour != null)
            currentBehaviour.UpdateBehaviour();

            
    }

    void FixedUpdate()
    {
        if (heuristics)
            return;

        timer += Time.fixedDeltaTime;
        if (timer > 1f)
        {
            RequestDecision();
            RequestAction();
            timer = 0f;
        }
        
        EnergyUpdate();
    }

    void EnergyUpdate()
    {
        if (colonist.energy > 0f)
            colonist.energy -= Time.fixedDeltaTime/60f;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(colonist.traits.laziness);
        sensor.AddObservation(colonist.traits.nature);
        sensor.AddObservation(colonist.traits.speed);
        sensor.AddObservation(colonist.traits.attackStrength);
        sensor.AddObservation(colonist.traits.mineStrength);
        sensor.AddObservation(colonist.traits.healing);

        sensor.AddObservation((int)colonist.state);

        sensor.AddObservation(colonist.health);
        sensor.AddObservation(colonist.energy);

        /*
        List<float> distances = GetStateDistances();
        foreach (float distance in distances)
        {
            sensor.AddObservation(distance);
        }
        */
        ColonistArea area = ColonyHandler.Instance.GetArea(areaIndex);
        sensor.AddObservation(area.colony.wealth);
        sensor.AddObservation(area.colony.food);
    }

    //TODO: Make reward cumulative across all agents. (look up SharedReward() ML agents method)
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var state = actionBuffers.DiscreteActions[0];
        SetState(state);
        var reproduce = actionBuffers.DiscreteActions[1];
        switch (reproduce)
        {
            default:
                break;
            case 1:
                // need to check if we have enough colonists and resources
                break;
        }

        AddReward(colonist.CalculateReward());
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    public void SetState(int stateIn)
    {
        Colonist.State state = (Colonist.State)stateIn;
        if (state != colonist.state)
        {
            // changing states
            if (currentBehaviour != null)
                currentBehaviour.StopBehaviour();

            currentBehaviour = GetBehaviour(state);
            currentBehaviour.StartBehaviour();
        }
        colonist.state = state;
    }

    public ColonistStateBehaviour GetBehaviour(Colonist.State state)
    {
        foreach (ColonistStateBehaviour stateBehaviour in stateBehaviours)
        {
            if (stateBehaviour.state == state)
                return stateBehaviour;
        }
        return null;
    }

    public List<float> GetStateDistances()
    {
        /*
        List<float> vals = new List<float>();
        Colonist.State stateCache = colonist.state;
        foreach(Colonist.State state in Enum.GetValues(typeof(Colonist.State)))
        {
            if (state == Colonist.State.Idle)
                vals.Add(-1f);
            
            colonist.state = state;
            vals.Add(Vector3.Distance(transform.position, GetTargetFunc(colonist)));
        }
        colonist.state = stateCache;
        return vals;
        */
        return null;
    }

    public void Die()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc(this);

        Destroy(this.gameObject);
    }

    public void Damage(int val)
    {
        float damage = (float)val/100f;
        colonist.health -= damage;
        Debug.Log("Colonist Damage: " + val + ", " + colonist.health);
        if (colonist.health <= 0)
        {
            // die
            Die();
        }
    }
    public void DestroyAgent()
    {
        Destroy(this.gameObject);
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

}

[Serializable]
public class Colonist
{
    public float health;
    public float energy;
    public Vector3 spawnPosition;

    public ColonistTraits traits;

    public enum State
    {
        Idle,
        Rest,
        Mine,
        Patrol,
        Heal,
        Collect,
    }
    public State state;

    List<RewardWeight> weights;

    public void Initialize()
    {
        health = 1.0f;
        energy = 1.0f;

        traits = new ColonistTraits();
        traits.Randomize(0.5f);

        weights = ColonyHandler.Instance.weights;
    }

    public float CalculateReward()
    {
        float healthReward = health - 0.5f; // anything below half health will be seen as a negative reward
        if (healthReward > 0f)
            healthReward += 0.5f;
        float energyReward = energy = 0.5f;
        if (energyReward > 0f)
            energyReward += 0.5f; 
        return ((weights[0]._val * healthReward) + (weights[1]._val * energyReward)) / weights.Count;
    }
}

[Serializable]
public class ColonistTraits
{
    public float speed = 1f;
    public float laziness = 1f;
    public float nature = 1f;
    public float healing = 1f;
    public float attackStrength = 1f;
    public float mineStrength = 1f; //"dwarfiness" - Alex 2023

    public void Randomize(float val)
    {
        speed = UnityEngine.Random.Range(val, 1f);
        laziness = UnityEngine.Random.Range(val, 1f);
        nature = UnityEngine.Random.Range(val, 1f);
        healing = UnityEngine.Random.Range(val, 1f);
        attackStrength = UnityEngine.Random.Range(val, 1f);
        mineStrength = UnityEngine.Random.Range(val, 1f);
    }
}

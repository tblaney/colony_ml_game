using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;
using Utils;

public class ColonistAgent : Agent, IEnemy, ITarget
{
    [Header("Inputs:")]
    public bool heuristics;
    List<ColonistStateBehaviour> stateBehaviours;

    [Header("Debug:")]
    public Colonist colonist;
    public ColonistStateBehaviour currentBehaviour;
    public float timer;
    Action<ColonistAgent> OnDestroyFunc;
    public Action OnActionsFunc;

    //--------------------------------------------------------------
    public override void Initialize()
    {

    }
    public void Setup(Colonist colonist, Action<ColonistAgent> OnDestroyFunc, Colonist.State state)
    {
        this.colonist = colonist;
        this.OnDestroyFunc = OnDestroyFunc;

        ColonistStateBehaviour[] behaviours = GetComponents<ColonistStateBehaviour>();
        stateBehaviours = new List<ColonistStateBehaviour>();
        foreach (ColonistStateBehaviour behaviour in behaviours)
        {
            behaviour.Initialize();
            stateBehaviours.Add(behaviour);
        }

        if (heuristics)
        {
            SetState((int)state);
        }

        RequestDecision();
    }
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (!Application.isPlaying)
            return;
        
        if (currentBehaviour != null)
            currentBehaviour.UpdateBehaviour();

        if (heuristics)
            return;

        timer += Time.fixedDeltaTime;
        if (timer > 5f)
        {
            //if (colonist.state == Colonist.State.Heal | colonist.state == Colonist.State.Patrol)
            if (colonist.state == Colonist.State.Heal)
                RequestDecision();
            //RequestAction();
            timer = 0f;
        }
        
        EnergyUpdate();
    }
    void EnergyUpdate()
    {
        if (colonist.energy > 0f)
            colonist.energy -= Time.fixedDeltaTime/180f;

        if (colonist.energy <= 0f)
        {
            //Damage(1);
        }
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

        ColonistArea area = ColonyHandler.Instance.area;
        sensor.AddObservation(area.colony.wealth);
        sensor.AddObservation(area.colony.food);
        //sensor.AddObservation(area.colony.colonists.Count);
        sensor.AddObservation(ColonyHandler.Instance.GetColonistAmount());
        sensor.AddObservation(HabitationHandler.Instance.GetBotAmountToColonist(this));

        List<float> distances = GetStateDistances();
        foreach (float distance in distances)
        {
            sensor.AddObservation(distance);
        }
    }
    //TODO: Make reward cumulative across all agents. (look up SharedReward() ML agents method)
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var state = actionBuffers.DiscreteActions[0];
        AddReward(GetBehaviour((Colonist.State)state).CalculateDecisionReward());
        SetState(state);
        AddReward(colonist.CalculateReward());
        if (OnActionsFunc != null)
        {
            OnActionsFunc();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
    public void SetState(int stateIn)
    {
        Debug.Log("Colonist Set State: " + this.gameObject + ", " + stateIn);
        Colonist.State state = (Colonist.State)stateIn;
        if (state != colonist.state)
        {
            AddReward(-0.05f);
            // changing states
            if (currentBehaviour != null)
                currentBehaviour.StopBehaviour();

            currentBehaviour = GetBehaviour(state);
            currentBehaviour.StartBehaviour();
        } else
        {
            //f (currentBehaviour != null)
            //    currentBehaviour.StartBehaviour();
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
        // TODO: get a list of distances to targets in each state
        List<float> vals = new List<float>();
        foreach(Colonist.State state in Enum.GetValues(typeof(Colonist.State)))
        {
            if (state == Colonist.State.Idle)
                vals.Add(-1f);
            
            vals.Add(GetBehaviour(state).GetStateDistance());
        }
        return vals;
    }
    public void Die()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc(this);

        Destroy(this.gameObject);
    }
    public bool Damage(int val)
    {
        colonist.vitality.Damage(val);
        colonist.RefreshHealth();
        // if colonist heals above max health then set colonist health to max
        if (colonist.health > 1.0f) {
            colonist.health = 1.0f;
        }
        //Debug.Log("Colonist Damage: " + val + ", " + colonist.health);
        if (colonist.health <= 0)
        {
            // die
            AddReward(-5f);
            Die();
            return false;
        } else
        {
            RequestDecision();
        }
        return true;
    }
    public bool IsInjured() {
        if (colonist.health < 1.0f) {
            return true;
        } else {
            return false;
        }
    }
    public void Energize(int val) {
        float addedEnergy = (float)val/100f;
        colonist.energy += addedEnergy;
        if (colonist.energy > 1.0f){
            colonist.energy = 1.0f;
        }
        //Debug.Log("Colonist Energy: " + val + ", " + colonist.energy);
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
    public Vitality GetVitality()
    {
        return colonist.vitality;
    }
    public string[] GetStrings()
    {
        return ColonyHandler.Instance.GetStrings();
    }
    public ITarget GetTarget()
    {
        return this;
    }
}

[Serializable]
public class Colonist
{
    public Vitality vitality;
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

    //List<RewardWeight> weights;

    public void Initialize()
    {
        vitality = new Vitality(){_name = "health", _val = 100};
        health = 1.0f;
        energy = 1.0f;

        traits = new ColonistTraits();
        traits.Randomize(0.5f);
    }

    public void RefreshHealth()
    {
        health = vitality.GetVitalityNormalized();
    }
    

    public float CalculateReward()
    {
        float reward = 0f;
        if (health < 0.2f)
        {
            reward -= 1f;
        }
        if (energy < 0.2f)
        {
            reward -= 1f;
        }
        
        return reward;
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

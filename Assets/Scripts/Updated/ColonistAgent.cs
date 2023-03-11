using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;

public class ColonistAgent : Agent
{
    public int areaIndex = 0;
    public Colonist colonist;

    public float timer;

    //public int 

    public override void Initialize()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            RequestDecision();
            RequestAction();
            timer = 0f;
        }
    }

    public void Setup(Colonist colonist, int index)
    {
        this.colonist = colonist;
        areaIndex = index;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((int)colonist.state);
        sensor.AddObservation(colonist.health);
        sensor.AddObservation(colonist.energy);
        sensor.AddObservation(colonist.traits.laziness);
        sensor.AddObservation(colonist.traits.nature);
        sensor.AddObservation(colonist.traits.speed);
        sensor.AddObservation(colonist.traits.attackStrength);
        sensor.AddObservation(colonist.traits.mineStrength);
        sensor.AddObservation(colonist.traits.healing);
        
    }

    public void Damage(float amount) {
        colonist.health -= amount;
    }

    //TODO: Make reward cumulative across all agents. (look up SharedReward() ML agents method)
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

}


[Serializable]
public class Colonist
{
    public float health;
    public float energy;
    public bool dead;

    public ColonistTraits traits;
    
    public enum State
    {
        Idle,
        Rest,
        Mine,
        Defend,
        Heal,
        Collect,
        Flee,
    }
    public State state;

    public Colonist()
    {
        health = 1.0f;
        energy = 1.0f;

        traits = new ColonistTraits();
        traits.Randomize(0.5f);
    }
}

[Serializable]
public class ColonistTraits
{
    public float speed;
    public float laziness;
    public float nature;
    public float healing;
    public float attackStrength;
    public float mineStrength; //"dwarfiness" - Alex 2023

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;


public class ColonistArea : MonoBehaviour
{
    [Header("Inputs:")]
    public Colony colony;
    public ColonyParameters parameters;
    [Header("Agent Inputs:")]
    public GameObject agentPrefab;
    public bool _active = false;
    
    MeshRenderer planeRest;
    SimpleMultiAgentGroup agentGroup;
    List<ColonistAgent> colonistAgents;

    //---setup---//
    void Awake()
    {
        agentGroup = new SimpleMultiAgentGroup();
        colonistAgents = new List<ColonistAgent>();
    }
    public void Initialize(MeshRenderer planeRest)
    {
        this.planeRest = planeRest;
        Reset();
        _active = true;
    }
    public void Reset()
    {
        CancelInvoke();
        ClearAll();

        colony.wealth = 0;
        colony.food = 0;
        SetupColonists();
        // spawn agents
        foreach (Colonist colonist in colony.colonists)
        {
            SpawnColonist(colonist);
        }
    }   
    void SetupColonists()
    {
        colony.colonists = new List<Colonist>();
        // randomly generate starting colonistAgents
        CreateAgentsWithState(parameters.colonistAmountStart, Colonist.State.Collect);
    }
    void CreateAgentsWithState(int numColonists, Colonist.State state)
    {
        for (int i = 0; i < numColonists; i++)
        {
            Colonist colonist = new Colonist(){};
            colonist.Initialize();
            colony.colonists.Add(colonist);
        }
    }
    void ClearAll()
    {
        // clear agents and enemies
        foreach (ColonistAgent agent in colonistAgents)
        {
            agentGroup.UnregisterAgent(agent);
            agent.DestroyAgent();
        }
        colonistAgents.Clear();
    }
    void RequestAgentDecision()
    {
        foreach (ColonistAgent agent in colonistAgents)
        {
            agent.RequestDecision();
        }
    }
    //---processing---//
    public void SpawnColonist(Colonist colonist, Vector3 position = default(Vector3))
    {
        if (position != default(Vector3))
        {
            colonist.spawnPosition = position;
        } else
        {
            colonist.spawnPosition = GetRestZone();
        }
        GameObject obj = Instantiate(agentPrefab, colonist.spawnPosition, Quaternion.identity, this.transform);
        ColonistAgent agent = obj.GetComponent<ColonistAgent>();
        //agent.OnActionsFunc = () => { AddGroupReward(colony.CalculateReward()); };
        agent.Setup(colonist, DestroyColonist, Colonist.State.Collect);
        agentGroup.RegisterAgent(agent);
        colonistAgents.Add(agent);
    }
    public void DestroyColonist(ColonistAgent agent)
    {
        AddGroupReward(-5f);
        if (colonistAgents.Contains(agent))
        {
            colonistAgents.Remove(agent);
            agentGroup.UnregisterAgent(agent);
            colony.colonists.Remove(agent.colonist);
        }
        if (colonistAgents.Count <= 1)
        {
            // can no longer reproduce so we must fail the game
            AddGroupReward(-10f);
            _active = false;
        }
    }
    //---gets---//
    public ColonistAgent GetClosestColonist(Vector3 position, bool healthCheck = false)
    {
        float distance = 10000f;
        ColonistAgent colonistAgent = null;
        foreach (ColonistAgent agent in colonistAgents)
        {
            if (healthCheck && agent.colonist.health >= 1.0f)
                continue;
            
            float distanceAgent = Vector3.Distance(agent.GetPosition(), position);
            if (distanceAgent < distance)
            {
                distance = distanceAgent;
                colonistAgent = agent;
            }
        }
        return colonistAgent;
    }
    public Vector3 GetRestZone()
    {
        Bounds restBounds = planeRest.bounds;
        float x = UnityEngine.Random.Range(restBounds.min.x, restBounds.max.x);
        float z = UnityEngine.Random.Range(restBounds.min.z, restBounds.max.z);
        Vector3 position_new = new Vector3(x, 30f, z);
        return position_new;
    }
    //---ML---//
    public void AddGroupReward(float val)
    {
        if (agentGroup != null)
            agentGroup.AddGroupReward(val);
    }
    //---inventory---//
    public void AddWealth(int amount)
    {
        colony.wealth += amount;
        AddGroupReward(1f);
    }
    public void AddFood(int amount)
    {
        colony.food += amount;
        AddGroupReward(0.5f);
        FoodCheck();
    }
    public void UseFood(int amount)
    {
        colony.food -= amount;
        if (colony.food < 0)
            colony.food = 0;
    }
    void FoodCheck()
    {
        //if food collected > threshold, new agent spawns
        if (colony.food > parameters.foodThreshold && colony.colonists.Count < parameters.colonistAmountMax)
        {
            colony.food -= (int)parameters.foodThreshold;
            Colonist colonist = new Colonist(){};
            colonist.Initialize();
            SpawnColonist(colonist);
            AddGroupReward(0.5f);
        }
    }
    public List<ColonistAgent> GetColonistAgents()
    {
        return colonistAgents;
    }
}


[Serializable]
public struct Colony
{
    public int wealth;
    public int food;
    public List<Colonist> colonists;

}

[Serializable]
public class ColonyParameters
{   
    public int colonistAmountStart = 5;
    public int colonistAmountMax = 20;
    public float foodThreshold = 5; //food to be collected to spawn new agent
}
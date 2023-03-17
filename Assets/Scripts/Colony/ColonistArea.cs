using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;


public class ColonistArea : MonoBehaviour
{
    [Header("Inputs:")]
    public int areaIndex = 1;
    public Colony colony;
    [Header("Agent Inputs:")]
    public GameObject agentPrefab;

    [Header("Enemy Inputs:")]
    public GameObject enemyPrefab;   
    public List<Transform> enemySpawnLocations;

    [Space(20)]
    public MeshRenderer planeRest;


    SimpleMultiAgentGroup agentGroup;
    ColonistProcessor processor;

    List<ColonistAgent> colonistAgents;
    List<EnemyAgent> enemyAgents;

    //---setup---//
    void Awake()
    {
        agentGroup = new SimpleMultiAgentGroup();
        processor = GetComponent<ColonistProcessor>();
        colonistAgents = new List<ColonistAgent>();
        enemyAgents = new List<EnemyAgent>();
    }
    public void Initialize()
    {
        processor.Initialize(areaIndex);
        Processor processorFood = processor.GetProcessor("food");
        processorFood.amount = ColonyHandler.parameters.foodAmount;
        Processor processorMinerals = processor.GetProcessor("minerals");
        processorMinerals.amount = ColonyHandler.parameters.mineralAmount;
    }
    void SetupColonists()
    {
        colony.colonists = new Dictionary<Colonist, Colonist.State>();
        // randomly generate starting colonistAgents
        CreateAgentsWithState(ColonyHandler.parameters.collectAmountStart, Colonist.State.Collect);
        CreateAgentsWithState(ColonyHandler.parameters.mineAmountStart, Colonist.State.Mine);
        CreateAgentsWithState(ColonyHandler.parameters.patrolAmountStart, Colonist.State.Patrol);
        CreateAgentsWithState(ColonyHandler.parameters.restAmountStart, Colonist.State.Rest);
        CreateAgentsWithState(ColonyHandler.parameters.idleAmountStart, Colonist.State.Idle);
        CreateAgentsWithState(ColonyHandler.parameters.healAmountStart, Colonist.State.Heal);
    }
    void CreateAgentsWithState(int numColonists, Colonist.State state)
    {
        for (int i = 0; i < numColonists; i++)
        {
            Colonist colonist = new Colonist(){};
            colonist.Initialize();
            colony.colonists.Add(colonist, state);
        }
    }
    public void Reset()
    {
        Debug.Log("Colonist Area Reset");

        CancelInvoke();
        processor.Reset();
        ClearAll();

        // called at the beginning of the game, so we spawn everything in here
        colony.wealth = 0;
        colony.food = 0;
        SetupColonists();
        // spawn agents
        foreach (KeyValuePair<Colonist, Colonist.State> colonist in colony.colonists)
        {
            SpawnColonist(colonist.Key, colonist.Value);
        }

        InvokeRepeating("SpawnEnemyRepeating", UnityEngine.Random.Range(0f, ColonyHandler.parameters.enemySpawnRate), ColonyHandler.parameters.enemySpawnRate);
    }   
    public void RefreshInactive()
    {
        processor.RefreshInactive();
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
        foreach (EnemyAgent agent in enemyAgents)
        {
            agent.DestroyAgent();
        }
        enemyAgents.Clear();
    }
    void SpawnEnemyRepeating()
    {
        if (enemyAgents.Count >= ColonyHandler.parameters.enemyAmountMax)
            return;
        
        SpawnEnemy();
    }
    void RequestAgentDecision()
    {
        foreach (ColonistAgent agent in colonistAgents)
        {
            agent.RequestDecision();
        }
    }
    //---processing---//
    public void SpawnColonist(Colonist colonist, Colonist.State state, Vector3 position = default(Vector3))
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
        agent.Setup(colonist, areaIndex, DestroyColonist, state);
        agentGroup.RegisterAgent(agent);
        colonistAgents.Add(agent);
    }
    public void DestroyColonist(ColonistAgent agent)
    {
        AddGroupReward(ColonyHandler.Instance.GetReward("agent death"));

        if (colonistAgents.Contains(agent))
        {
            colonistAgents.Remove(agent);
            agentGroup.UnregisterAgent(agent);
            colony.colonists.Remove(agent.colonist);
        }
        if (colonistAgents.Count <= 1)
        {
            // can no longer reproduce so we must fail the game
            AddGroupReward(ColonyHandler.Instance.GetReward("colony failure"));
            ColonyHandler.Instance.Reset(areaIndex);
        }
    }
    public void SpawnEnemy()
    {
        if (enemyAgents.Count >= ColonyHandler.parameters.enemyAmountMax)
            return;
        //generate random enemy
        Enemy enemy = new Enemy();
        enemy.InitializeRandom();
        GameObject obj = Instantiate(enemyPrefab, enemySpawnLocations[UnityEngine.Random.Range(0, enemySpawnLocations.Count)].position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, this.transform);
        EnemyAgent agent = obj.GetComponent<EnemyAgent>();
        agent.Setup(enemy, areaIndex, DestroyEnemy);
        enemyAgents.Add(agent);

        RequestAgentDecision();
    }

    public void DestroyEnemy(EnemyAgent agent)
    {
        if (enemyAgents.Contains(agent))
        {
            enemyAgents.Remove(agent);
        }
        //AddGroupReward(ColonyHandler.Instance.GetReward("enemy death"));
        RequestAgentDecision();
    }
    //---gets---//
    public EnemyAgent GetClosestEnemy(Vector3 position)
    {
        float distance = 10000f;
        EnemyAgent enemyAgent = null;
        foreach (EnemyAgent agent in enemyAgents)
        {
            float distanceAgent = Vector3.Distance(agent.GetPosition(), position);
            if (distanceAgent < distance)
            {
                distance = distanceAgent;
                enemyAgent = agent;
            }
        }
        return enemyAgent;
    }
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
    public Collectible GetClosestCollectible(Collectible.Type type, Vector3 position)
    {
        //Debug.Log("Get Closest Collectible: " + type + ", " + position);
        string name = "";
        switch (type)
        {
            case Collectible.Type.Food:
                name = "food";
                break;
            case Collectible.Type.Mineral:
                name = "minerals";
                break;
        }
        Node node = processor.GetClosestNode(name, position);
        return node as Collectible;
    }
    public Vector3 GetRestZone()
    {
        Bounds restBounds = planeRest.bounds;
        float x = UnityEngine.Random.Range(restBounds.min.x, restBounds.max.x);
        float z = UnityEngine.Random.Range(restBounds.min.z, restBounds.max.z);
        Vector3 position_new = new Vector3(x, 0f, z);
        return position_new;
    }
    public int GetEnemyCount()
    {
        if (enemyAgents != null)
        {
            return enemyAgents.Count;
        }   
        return 0;
    }
    public int GetCollectibleCount(Collectible.Type type)
    {
        string name = "";
        switch (type)
        {
            case Collectible.Type.Food:
                name = "food";
                break;
            case Collectible.Type.Mineral:
                name = "minerals";
                break;
        }
        return processor.GetNodeCount(name);
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
        WealthCheck();

        AddGroupReward(5f);
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
    void WealthCheck()
    {
        // check for success state
        if (colony.wealth > colony.wealthThreshold)
        {
            // end game
            AddGroupReward(ColonyHandler.Instance.GetReward("colony success"));
            Reset();
        }
    }
    void FoodCheck()
    {
        //if food collected > threshold, new agent spawns
        if (colony.food > ColonyHandler.parameters.foodThreshold)
        {
            colony.food -= (int) ColonyHandler.parameters.foodThreshold;
            Colonist colonist = new Colonist(){};
            colonist.Initialize();
            SpawnColonist(colonist, Colonist.State.Collect);
            AddGroupReward(0.5f);
        }
    }
}


[Serializable]
public struct Colony
{
    public int wealth;
    public int wealthThreshold;
    public int food;
    public Dictionary<Colonist, Colonist.State> colonists;

    public float CalculateReward()
    {
        float reward = 0f;
        reward = wealth / wealthThreshold;
        return reward;
    }
}
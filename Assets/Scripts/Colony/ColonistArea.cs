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
        colony.colonists = new List<Colonist>();
        // randomly generate starting colonistAgents
        for (int i = 0; i < ColonyHandler.parameters.colonistAmountStart; i++)
        {
            Colonist colonist = new Colonist(){};
            colonist.Initialize();
            colony.colonists.Add(colonist);
        }
    }
    public void Reset()
    {
        // called at the beginning of the game, so we spawn everything in here
        colony.wealth = 0;
        colony.food = 0;

        SetupColonists();

        CancelInvoke();
        processor.Reset();
        ClearAll();
        // spawn agents
        foreach (Colonist colonist in colony.colonists)
        {
            SpawnColonist(colonist);
        }

        //InvokeRepeating("SpawnEnemyRepeating", ColonyHandler.parameters.enemySpawnRate, ColonyHandler.parameters.enemySpawnRate);
        InvokeRepeating("SpawnEnemyRepeating", 0f, ColonyHandler.parameters.enemySpawnRate);
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
        agent.Setup(colonist, areaIndex, DestroyColonist);
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
    }

    public void DestroyEnemy(EnemyAgent agent)
    {
        if (enemyAgents.Contains(agent))
        {
            enemyAgents.Remove(agent);
        }
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
            if (healthCheck && agent.colonist.health > 0.8f)
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
    }
    public void AddFood(int amount)
    {
        colony.food += amount;
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
}


[Serializable]
public struct Colony
{
    public int wealth;
    public int wealthThreshold;
    public int food;
    public List<Colonist> colonists;
}
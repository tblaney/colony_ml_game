using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;


public class ColonistArea : MonoBehaviour
{
    [Header("Inputs:")]
    public int areaIndex = 1;

    [Header("Agent Inputs:")]
    public GameObject agentPrefab;
    public GameObject enemyPrefab;

    ColonistProcessor processor;

    [Header("Enemy Inputs:")]
    List<ColonistAgent> colonistAgents;
    List<EnemyAgent> enemyAgents;

    public float resetThresholdMinutes;

    private float resetTimer;
    private int numresets;

    //---setup---//
    void Awake()
    {
        processor = GetComponent<ColonistProcessor>();
        colonistAgents = new List<ColonistAgent>();
        enemyAgents = new List<EnemyAgent>();
        resetTimer = 0;
        numresets = 0;
    }
    public void Initialize()
    {
        processor.Initialize(areaIndex);
        Processor processorFood = processor.GetProcessor("food");
        processorFood.amount = ColonyHandler.parameters.foodAmount;
        Processor processorMinerals = processor.GetProcessor("minerals");
        processorMinerals.amount = ColonyHandler.parameters.mineralAmount;
        Processor processorWalls = processor.GetProcessor("walls");
        processorWalls.amount = ColonyHandler.parameters.wallAmount;
    }
    void SetupColonists()
    {
        for (int i = 0; i < ColonyHandler.parameters.colonistAmount; i++)
        {
            SpawnAgent();
        }
    }
    void SetupEnemies()
    {
        //number of enemies decreases with each reset
        float enemynum = ColonyHandler.parameters.enemyAmountMax - (10*numresets);
        if (enemynum <= ColonyHandler.parameters.enemyAmountMin)
        {
            numresets = 0;
        }
        for (int i = 0; i < enemynum; i++)
        {
            SpawnEnemy();
        }
    }
    public void SoftReset()
    {
        Debug.Log("Colonist Area Soft Reset");

        CancelInvoke();
        processor.Reset();
        //Do a soft clearall (don't delete colonistAgents)
        foreach (EnemyAgent agent in enemyAgents)
        {
            agent.DestroyAgent();
        }
        enemyAgents.Clear();

        SetupEnemies();
        //Move colonists to random new open position
        MoveColonists();

        //Restart enemy spawning
        if (ColonyHandler.parameters.enemyRepeatSpawning)
        {
            InvokeRepeating("SpawnEnemyRepeating", 0f, ColonyHandler.parameters.enemySpawnRate);
        }
        numresets += 1;
    }
    public void Reset()
    {   
        resetTimer = 0;
        Debug.Log("Colonist Area Reset");

        CancelInvoke();
        processor.Reset();
        ClearAll();

        SetupColonists();
        SetupEnemies();

        if (ColonyHandler.parameters.enemyRepeatSpawning)
        {
            InvokeRepeating("SpawnEnemyRepeating", 0f, ColonyHandler.parameters.enemySpawnRate);
        }
    }   
    public void RefreshInactive()
    {
        processor.RefreshInactive();
    }
    void ClearAll()
    {
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
    void MoveColonists()
    {
        foreach (ColonistAgent agent in colonistAgents)
        {
            Vector3 newPos = processor.GetOpenPosition();
            agent.gameObject.transform.position = newPos;
        }
    }
    void FixedUpdate()
    {
        resetTimer += Time.fixedDeltaTime;
        if (resetTimer >= resetThresholdMinutes*60 || enemyAgents.Count <= 0)
        {
            resetTimer = 0;
            SoftReset();
        }
    }
    //---processing---//
    public void SpawnAgent()
    {
        Vector3 randomPosition = processor.GetOpenPosition();
        GameObject obj = Instantiate(agentPrefab, randomPosition, Quaternion.identity, this.transform.parent);
        ColonistAgent agent = obj.GetComponent<ColonistAgent>();
        agent.Setup(areaIndex);
        colonistAgents.Add(agent);
    }

    //---gets---//
    public ColonistAgent GetClosestColonist(Vector3 position, bool healthCheck = false)
    {
        // for enemies
        float distance = 10000f;
        ColonistAgent colonistAgent = null;
        foreach (ColonistAgent agent in colonistAgents)
        {
            //if (healthCheck)
            //    continue;
            
            float distanceAgent = Vector3.Distance(agent.GetPosition(), position);
            if (distanceAgent < distance)
            {
                distance = distanceAgent;
                colonistAgent = agent;
            }
        }
        return colonistAgent;
    }
    void SpawnEnemyRepeating()
    {
        if (enemyAgents.Count >= ColonyHandler.parameters.enemyAmountMax)
            return;
        SpawnEnemy();
    }
    void SpawnEnemy()
    {
        if (enemyAgents.Count >= ColonyHandler.parameters.enemyAmountMax)
            return;
        Enemy enemy = new Enemy();
        enemy.InitializeRandom();
        Vector3 randomPosition = processor.GetOpenPosition();
        GameObject obj = Instantiate(enemyPrefab, randomPosition, Quaternion.identity, this.transform);
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
        // foreach (ColonistAgent colonist in colonistAgents)
        // {
        //     //small reward assigned collectively for destruction of an enemy
        //     colonist.AddReward(0.25f);
        // }
    }
    public Collectible GetClosestCollectible(Collectible.Type type, Vector3 position)
    {
        // just in case we add vector observations
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
}
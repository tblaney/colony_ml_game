using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;


public class ColonistArea : MonoBehaviour
{
    [Header("Inputs:")]
    public int areaIndex = 1;
    public GameObject agentPrefab;

    ColonistProcessor processor;
    List<ColonistAgent> colonistAgents;

    //---setup---//
    void Awake()
    {
        processor = GetComponent<ColonistProcessor>();
        colonistAgents = new List<ColonistAgent>();
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
        // TODO: spawn agents
        for (int i = 0; i < ColonyHandler.parameters.colonistAmount; i++)
        {
            SpawnAgent();
        }
    }
    public void Reset()
    {
        Debug.Log("Colonist Area Reset");

        CancelInvoke();
        processor.Reset();
        ClearAll();

        SetupColonists();

        //InvokeRepeating("SpawnEnemyRepeating", UnityEngine.Random.Range(0f, ColonyHandler.parameters.enemySpawnRate), ColonyHandler.parameters.enemySpawnRate);
    }   
    public void RefreshInactive()
    {
        processor.RefreshInactive();
    }
    void ClearAll()
    {
        // TODO: clear agents, enemies
    }
    void SpawnEnemyRepeating()
    {
        // TODO: spawn enemy 
    }

    //---processing---//
    public void SpawnAgent()
    {
        // TODO: spawn agent
        Vector3 randomPosition = processor.GetOpenPosition();
        GameObject obj = Instantiate(agentPrefab, randomPosition, Quaternion.identity, this.transform.parent);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class ColonistArea : MonoBehaviour
{
    public int startingAgentAmount;
    public int maxAgentAmount;

    public int foodSpawnAmount;
    public int rockSpawnAmount;

    public List<Colonist> colonists;

    public Transform agentPrefab;
    public List<Transform> enemyPrefabs;
    public List<Transform> enemySpawnPoints;
    public List<Transform> rockPrefabs;
    public Transform foodPrefab;

    public Bounds restBounds;

    public int areaIndex;
    public int wealth;
    public int wealthThreshold;
    public int foodAmount;

    public List<BlockController> activeRocks;
    public List<BlockController> activeFood;

    SimpleMultiAgentGroup agentGroup;

    void Awake()
    {
        agentGroup = new SimpleMultiAgentGroup();
    }


    public void AddGroupReward(float val)
    {
        if (agentGroup != null)
            agentGroup.AddGroupReward(val);
    }

    void SpawnFood()
    {

    }

    void SpawnRocks()
    {

    }

    void SetupAgents()
    {
        colonists = new List<Colonist>();
        // randomly generate starting agents
        for (int i = 0; i < maxAgentAmount; i++)
        {
            Colonist colonist = new Colonist(){};
            colonists.Add(colonist);
            if (i < startingAgentAmount)
            {
                colonist.dead = false;
            } else
            {
                colonist.dead = true;
            }
        }
    }

    void ResetArea()
    {
        foreach (Colonist colonist in colonists)
        {
            SpawnAgent(colonist);
        }
    }


    void SpawnAgent(Colonist colonist)
    {
        Transform agent = Instantiate(agentPrefab, this.transform);
        ColonistAgent colonistAgent = agent.GetComponent<ColonistAgent>();
        colonistAgent.Setup(colonist, areaIndex);
    }

    void SpawnEnemy()
    {
        int idx = UnityEngine.Random.Range(0, enemySpawnPoints.Count);
        Transform prefab = enemyPrefabs[idx];

        Transform enemy = Instantiate(prefab, this.transform);
        // enemy logic:
    }

    public void AddWealth(int amount)
    {
        wealth += amount;
        WealthCheck();
    }

    public void AddFood(int amount)
    {
        foodAmount += amount;
    }

    public void UseFood(int amount)
    {
        foodAmount -= amount;
        if (foodAmount < 0)
            foodAmount = 0;
    }

    void WealthCheck()
    {
        // check for success state
        if (wealth > wealthThreshold)
        {
            // end game
            AddGroupReward(10);
            ResetArea();
        }
    }
}

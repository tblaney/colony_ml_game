using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Random = UnityEngine.Random;

public class ColonistArea : MonoBehaviour
{
    public List<Bounds> enemySpawns;

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

    public int enemyResources;
    public GameObject enemy;
    public LayerMask mask;

    private List<Vector3> enemySpawnVecs;
    private List<Vector3> openVecs;

    void Awake()
    {   
        // TODO: change this to bounds of enemy spawn locations.
        // Maybe also make enemySpawnVecs a list of lists. Currently enemies in a single group will each randomly spawn at any available spawn position.
        // Figured it would add to variation in challenge wiht enemies coming from different angles and also wasn't sure how to do a list of lists of vecs.
        foreach(Bounds spawnBounds in enemySpawns) {
            float xmin = spawnBounds.min.x;
            float xmax = spawnBounds.max.x;
            float zmin = spawnBounds.min.z;
            float zmax = spawnBounds.max.z;
            float y = spawnBounds.center.y;
            for(int x=(int)xmin; x<(int)xmax; x++){
                for(int z=(int)zmin; z<(int)zmax; z++){
                    enemySpawnVecs.Add(new Vector3(x+0.5f, y+0.5f, z+0.5f));
                }
            }
        }

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
        int intResources = enemyResources;
        // must have at least 1 enemy and must leave 3 resources for at least speed 1 damage 1 health 1
        int enemyNumbers = Random.Range(1, intResources - 3);
        // convert to float once enemyNumbers have been selected
        float resources = (float) (intResources - enemyNumbers);

        //Get appropriate enemy prefab for number of enemies
        Transform prefab = enemyPrefabs[enemyNumbers];

        // must have at least 1 speed and must leave 2 resources for at least damage 1 health 1
        float enemySpeed = Random.Range(1f, resources - 2f);
        resources = resources - enemySpeed;
        // must have at least 1 speed and must leave 1 resources for at least health 1
        float enemyDamage = Random.Range(1f, resources - 1f);
        resources = resources - enemyDamage;
        float enemyHealth = Random.Range(1f, resources);

        //Get open positions 
        openVecs = new List<Vector3>();
        // TODO: not sure what these float values are doing. Stole them from CreateFood in ColonyArea.cs. They're hard coded and may need to be changed.
        for (float f = 1.1f; f >0.3f; f-=0.1f)
        {
            foreach (Vector3 vec in enemySpawnVecs)
            {
                var hit = Physics.OverlapBox(vec, new Vector3(f, f, f), Quaternion.identity, mask);
                if (hit.Length > 0)
                {
                    continue;
                } 
                openVecs.Add(vec);
            }
            if (openVecs.Count > 1)
            {
                break;
            } else
            {
                openVecs.Clear();
            }
        }

        for (int i=0; i<enemyNumbers; i++){
            int index = UnityEngine.Random.Range(0, openVecs.Count);
            Vector3 newpos = openVecs[index];
            openVecs.RemoveAt(index);
            Transform newEnemy = Instantiate(prefab, this.transform);
            newEnemy.position = newpos;
            EnemyAgent enemyAgent = newEnemy.gameObject.GetComponent<EnemyAgent>();
            enemyAgent.Setup(areaIndex, enemyHealth, enemySpeed, enemyDamage);
        }
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

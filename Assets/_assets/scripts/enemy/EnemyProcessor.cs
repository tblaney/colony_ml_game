using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyProcessor : MonoBehaviour
{
    [Header("Inputs:")]
    public List<EnemyPrefab> _prefabs;
    public List<MeshRenderer> _spawnAreas;
    public NodeProcessor _nodeProcessor;
    public float enemySpawnTime;
    public float enemySpawnDev;
    public string defaultEnemyType;

    private float enemySpawnTimer;
    private float spawnThreshold;
    
    Habitation _habitation;

    public void Initialize()
    {
        enemySpawnTimer = 0;
        spawnThreshold = enemySpawnTime + UnityEngine.Random.Range(-enemySpawnDev, enemySpawnDev);
    }
    public void Setup(Habitation habitation, NodeProcessor nodeProcessor)
    {
        _habitation = habitation;
        _nodeProcessor = nodeProcessor;

    }
    void FixedUpdate()
    {
        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer >= spawnThreshold)
        {
            SpawnEnemies();
            enemySpawnTimer = 0;
            spawnThreshold = enemySpawnTime + UnityEngine.Random.Range(-enemySpawnDev, enemySpawnDev);
        }
    }
    void SpawnEnemies()
    {
        int spawnResources = _habitation._bots.Count;
        List<string> enemiesToSpawn = GetEnemyList(spawnResources);
        int spawnPlaneId = UnityEngine.Random.Range(0, _spawnAreas.Count);
        foreach(string enemyType in enemiesToSpawn)
        {
            Vector3Int position = _nodeProcessor.GetOpenPosition(_spawnAreas[spawnPlaneId]);
            Quaternion rotation = Quaternion.identity;
            SpawnEnemy(enemyType, position, rotation);
        }
    }
    void SpawnEnemy(string enemyType, Vector3Int position, Quaternion rotation)
    {   
        GameObject enemyPrefab = GetEnemyPrefab(enemyType);
        GameObject obj = Instantiate(enemyPrefab, position, rotation, this.transform);
        EnemyAgent agent = obj.GetComponent<EnemyAgent>(); 
        agent.Setup(DestroyEnemy);
        HabitationHandler.Instance._enemies.Add(agent);
    }
    public void DestroyEnemy(EnemyAgent enemy)
    {
       HabitationHandler.Instance._enemies.Remove(enemy);
        enemy.DestroyAgent();
    }
    public GameObject GetEnemyPrefab(string type)
    {
        foreach (EnemyPrefab prefab in _prefabs)
        {
            if (prefab.type == type)
            {
                return prefab._prefab;
            }
        }
        return null;
    }
    public List<string> GetEnemyList(int spawnResources)
    {  
        List<string> enemyList = new List<string>();
        int passes = 0;
        //iterate through all enemy prefabs a few times, rolling randomly to see if each enemy is spawned. 
        //The more dangerous an enemy the lower the spawn chance it should have.
        while (spawnResources > 0 || passes < 5)
        {
            foreach (EnemyPrefab enemy in _prefabs)
            {
                float randomRoll = UnityEngine.Random.Range(0f, 1f);
                //must pay cost to spawn each enemy. Default enemy is worth 1 cost. Cost is proportional to number of agents in colony.
                if (enemy.cost <= spawnResources && randomRoll <= enemy.spawnProbability)
                {
                    enemyList.Add(enemy.type);
                    spawnResources -= enemy.cost;
                }
            }
            passes += 1;
        }
        // if we still have spawnResources left over pad the wave out with default agents.
        if (spawnResources > 0)
        {
            for (int i=0; i<spawnResources; i++)
            {
                enemyList.Add(defaultEnemyType);
            }
        }
        
        return enemyList;
    }
}
[Serializable]
public struct EnemyPrefab
{
    public string type;
    public int cost;
    public float spawnProbability;
    public GameObject _prefab;
}
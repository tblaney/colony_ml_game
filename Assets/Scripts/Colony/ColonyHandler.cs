using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;

public class ColonyHandler : MonoBehaviour
{
    public static ColonyHandler Instance;
    [Header("Inputs:")]
    public List<ColonistArea> areas;
    public List<RewardWeight> weights;
    public ColonyParameters parametersIn;

    public static ColonyParameters parameters;
    Dictionary<int, ColonistArea> areaDict;


    void Awake()
    {
        Instance = this;

        parameters = parametersIn;

        areaDict = new Dictionary<int, ColonistArea>();
        foreach (ColonistArea area in areas)
        {
            areaDict.Add(area.areaIndex, area);
        }
    }
    void Start()
    {
        foreach (ColonistArea area in areas)
        {
            area.Initialize();
        }
    }
    void RefreshResources()
    {
        foreach (ColonistArea area in areas)
        {
            area.RefreshInactive();
        }
    }
    void OnEnable()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
    }
    public void EnvironmentReset()
    {
        CancelInvoke();
        Debug.Log("Environment Reset");
        foreach (ColonistArea area in areas)
        {
            area.Reset();
        }
        InvokeRepeating("RefreshResources", parameters.resourceRefreshTime, parameters.resourceRefreshTime);
    }
    public void Reset(int index)
    {
        ColonistArea area = GetArea(index);
        if (area != null)
            area.Reset();
    }

    public void AddFood(int amount, int areaIdx)
    {
        ColonistArea area = GetArea(areaIdx);
        if (area == null)
            return;
        
        area.AddFood(amount);
    }
    public void RemoveFood(int amount, int areaIdx)
    {
        ColonistArea area = GetArea(areaIdx);
        if (area == null)
            return;
        
        area.UseFood(amount);
    }
    public void AddWealth(int amount, int areaIdx)
    {
        ColonistArea area = GetArea(areaIdx);
        if (area == null)
            return;
        
        area.AddWealth(amount);
    }
    public ColonistArea GetArea(int idx)
    {
        ColonistArea area;
        if (areaDict.TryGetValue(idx, out area))
        {
            return area;
        }
        return null;
    }
    public float GetReward(string name)
    {
        foreach (RewardWeight reward in weights)
        {
            if (reward._name == name)
                return reward._val;
        }
        return 0f;
    }
    public Collectible GetClosestCollectible(Collectible.Type type, int areaIndex, Vector3 position)
    {
        ColonistArea area = GetArea(areaIndex);
        return area.GetClosestCollectible(type, position);
    }
    public ColonistAgent GetClosestColonist(int areaIndex, Vector3 position)
    {
        ColonistArea area = GetArea(areaIndex);
        return area.GetClosestColonist(position);
    }
    public ColonistAgent GetClosestInjuredColonist(int areaIndex, Vector3 position)
    {
        ColonistArea area = GetArea(areaIndex);
        return area.GetClosestColonist(position, true);
    }
    public EnemyAgent GetClosestEnemy(int areaIndex, Vector3 position)
    {
        ColonistArea area = GetArea(areaIndex);
        return area.GetClosestEnemy(position);
    }
    public Vector3 GetRestPosition(int areaIndex) 
    {
        ColonistArea area = GetArea(areaIndex);
        Vector3 restPosition = area.GetRestZone();
        return restPosition;
    }
}

[Serializable]
public struct RewardWeight
{
    public string _name;
    //[Range(0f, 1f)]
    public float _val;
}

[Serializable]
public class ColonyParameters
{
    public int colonistAmountStart = 8;
    public int colonistAmountMax = 20;
    public int enemyAmountMax = 4;
    public float enemySpawnRate = 30f;
    public int foodAmount = 60;
    public int mineralAmount = 30; // will be clustered
    public float resourceRefreshTime = 60f;
}
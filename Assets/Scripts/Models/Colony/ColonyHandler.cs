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
    public ColonyParameters parametersIn;

    public static ColonyParameters parameters;

    void Awake()
    {
        Instance = this;

        parameters = parametersIn;
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
    public ColonistArea GetArea(int idx)
    {
        foreach (ColonistArea area in areas)
        {
            if (area.areaIndex == idx)
                return area;
        }
        return null;
    }
    public ColonistAgent GetClosestColonist(int areaIndex, Vector3 position)
    {
        ColonistArea area = GetArea(areaIndex);
        return area.GetClosestColonist(position);
    }
}


[Serializable]
public class ColonyParameters
{   
    public int colonistAmount = 20;
    public int enemyAmountMax = 4;
    public float enemySpawnRate = 30f;
    public int foodAmount = 60;
    public int mineralAmount = 30; // will be clustered
    public int wallAmount = 20; // will be clustered
    public float resourceRefreshTime = 60f;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;

public class ColonyHandler : MonoBehaviour, IHandler
{
    public static ColonyHandler Instance;
    [Header("Inputs:")]
    public string name;
    public string description;
    public ColonistArea area;

    public void Initialize()
    {
        Instance = this;
    }
    public void NewColony(MeshRenderer planeRest)
    {
        //area.Initialize(planeRest, ColonyDeathCallback);
    }
    public void AddFood(int amount)
    {
        if (area == null)
            return;
        
        area.AddFood(amount);
    }
    public void RemoveFood(int amount)
    {
        if (area == null)
            return;
        
        area.UseFood(amount);
    }
    public void AddWealth(int amount)
    {
        if (area == null)
            return;
        
        area.AddWealth(amount);
    }
    public ColonistAgent GetClosestColonist(Vector3 position)
    {
        return area.GetClosestColonist(position);
    }
    public List<ColonistAgent> GetColonistAgents()
    {
        return area.GetColonistAgents();
    }
    public ColonistAgent GetClosestInjuredColonist(Vector3 position)
    {
        return area.GetClosestColonist(position, true);
    }
    public Vector3 GetRestPosition() 
    {
        Vector3 restPosition = area.GetRestZone();
        return restPosition;
    }
    public int GetColonistAmount()
    {
        return area.colony.colonists.Count;
    }
    public string[] GetStrings()
    {
        return new string[] {name, description};
    }
}

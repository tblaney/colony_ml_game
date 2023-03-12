using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : Node
{
    public enum Type
    {
        Food,
        Mineral
    }
    [Space(20)]
    [Header("Inputs:")]
    public Type type;
    public int amount;
    public int health;


    public void DestroyCollectible()
    {
        // perform reward:
        switch (type)
        {
            case Type.Food:
                ColonyHandler.Instance.AddFood(areaIndex, amount);
                break;
            case Type.Mineral:
                ColonyHandler.Instance.AddWealth(areaIndex, amount);
                break;
        }
        //DestroyNode();
        Activate(false);
        time = Time.fixedTime + ColonyHandler.parameters.resourceRefreshTime;
        SetBusy(false);
    }

    public void Damage(int val)
    {
        //Debug.Log("Collectible Damage: " + val);
        health -= val;
        if (health <= 0)
        {
            DestroyCollectible();
        }
    }
}
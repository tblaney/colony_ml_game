using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Resource : Node
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

    int healthCache;

    void Start()
    {
        healthCache = health;
    }
    public void DestroyResource()
    {
        // perform logic:
        switch (type)
        {
            case Type.Food:

                break;
            case Type.Mineral:

                break;
        }
        health = healthCache;
        SetBusy(false);
        if (refreshable)
        {
            save.time = Time.fixedTime + HabitationProcessor._parameters._resourceRefreshTime*60f;
            Activate(false);
        } else
        {
            DestroyNode();
        }
    }
    public bool Damage(int val)
    {
        // return true if still alive
        health -= val;
        if (health <= 0)
        {
            DestroyResource();
            return false;
        }
        return true;
    }
}

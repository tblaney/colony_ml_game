using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabitationProcessor : MonoBehaviour
{
    [Header("Inputs:")]
    public Habitation _habitation;
    public HabitationParameters _parametersIn;
    public static HabitationParameters _parameters;


    void Awake()
    {
        _parameters = _parametersIn;
    }
    public void Initialize()
    {
        
    }
    //__load__//
    public void Load(Habitation habitation = null)
    {
        if (habitation != null)
        {
            _habitation = habitation;
        } else
        {
            _habitation = new Habitation();
            _habitation.NewHabitation();
        }
        UIHandler.Instance.InitializeHabitation(_habitation);
        SpawnHabitation();
    }
    //__spawn__//
    public void SpawnHabitation()
    {
        
    }
    public void SpawnBot(HabBot bot)
    {

    }
}

[Serializable]
public class HabitationParameters
{
    [Tooltip("number of hab-bots to spawn in at start of game")]
    public int _botAmountStart = 3;
    [Tooltip("time to refresh resources in minutes")]
    public int _resourceRefreshTime = 5;
}
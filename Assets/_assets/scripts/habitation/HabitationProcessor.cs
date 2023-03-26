using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabitationProcessor : MonoBehaviour
{
    [Header("Inputs:")]
    public HabBotProcessor _botProcessor;
    public HabitationParameters _parametersIn;
    public MeshRenderer _restBoundsDefault;
    [Header("Debug:")]
    public Habitation _habitation;
    public static HabitationParameters _parameters;


    void Awake()
    {
        _parameters = _parametersIn;
    }
    public void Initialize()
    {
        _botProcessor.Initialize();
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
            _habitation.NewHabitation(_restBoundsDefault.bounds);
        }
        _habitation.Initialize();
        Destroy(_restBoundsDefault.gameObject);
        UIHandler.Instance.InitializeHabitation(_habitation);
        SpawnHabitation();
    }
    //__spawn__//
    public void SpawnHabitation()
    {
        _botProcessor.Setup(_habitation);
    }
    //__habitation__//
    public void AddInventory(int index)
    {
        _habitation.AddInventory(index);
    }
    public void RemoveInventory(int index)
    {
        _habitation.RemoveInventory(index);
    }
    //__gets__//
    public Color GetBotColor(int index)
    {
        return _botProcessor.GetColor(index);
    }
    public Sprite GetStateSprite(HabBot.State state)
    {
        return _botProcessor.GetSprite(state);
    }
    public HabBotController GetClosestBot(Vector3 position)
    {
        return _botProcessor.GetClosestBot(position);
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
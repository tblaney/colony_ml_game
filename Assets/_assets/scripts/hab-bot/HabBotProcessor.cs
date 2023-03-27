using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotProcessor : MonoBehaviour
{
    /*
        because we have different behaviours based on states, & some of these states are ml-agents while others 
        are hardcoded, best to switch between prefab variants when switching between bot states
    */
    [Header("Inputs:")]
    public List<HabBotPrefab> _prefabs;
    public NodeProcessor _nodeProcessor;
    
    List<HabBotController> _controllers;
    Habitation _habitation;

    public void Initialize()
    {
        _controllers = new List<HabBotController>();
    }
    public void Setup(Habitation habitation, NodeProcessor nodeProcessor)
    {
        _habitation = habitation;
        _nodeProcessor = nodeProcessor;
        foreach (HabBot bot in _habitation._bots)
        {
            bot.OnStateChange += Bot_StateChange;
        }
        SpawnBots();
    }
    void SpawnBots()
    {
        foreach (HabBot bot in _habitation._bots)
        {
            //bot.SetState(bot._stateDefault);
            bot.SetState(HabBot.State.Idle);
        }
    }
    void SpawnBot(HabBot bot, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = GetPrefab(bot._state);
        GameObject obj = Instantiate(prefab, position, rotation, this.transform);
        HabBotController botController = obj.GetComponent<HabBotController>();
        botController.Initialize(bot, DestroyBot);
        _controllers.Add(botController);
    }
    void DestroyBot(HabBotController controller)
    {
        if (_controllers.Contains(controller))
            _controllers.Remove(controller);
    }
    public void SetBotState(HabBot bot, HabBot.State state)
    {
        HabBotController controller = GetController(bot);
        if (controller != null)
            controller.DestroyBot();
    }
    // events
    private void Bot_StateChange(object sender, HabBot.StateChangeEventArgs e)
    {
        // we basically need to get the associated spawned bot, destroy it, and respawn it in same position/rotation
        HabBot bot = e._bot;
        HabBotController botController = GetController(bot);
        //Debug.Log("Bot State Change");
        BuiltNodeObject obj = _nodeProcessor.GetClosestNodeObject(Node.Type.Building, e._bot._position, 6) as BuiltNodeObject;
        Debug.Log("Bot State Change: " + obj);
        Vector3 position = (obj._behaviour as BuiltNodeBehaviourRestMachine).GetZonePosition();
        position.y = 30f;
        Quaternion rotation = Quaternion.identity;
        if (botController)
        {
            position = botController.transform.position;
            rotation = botController.transform.rotation;
            botController.DestroyBot();
        }
        SpawnBot(bot, position, rotation);
    }
    // gets
    public HabBotController GetController(HabBot bot)
    {
        foreach (HabBotController controller in _controllers)
        {
            if (controller.GetBot() == bot)
                return controller;
        }
        return null;
    }
    public GameObject GetPrefab(HabBot.State state)
    {
        foreach (HabBotPrefab prefab in _prefabs)
        {
            if (prefab._state == state)
                return prefab._prefab;
        }
        return null;
    }
    public HabBotController GetClosestBot(Vector3 position)
    {
        float distanceMin = 1000f;
        HabBotController controller = null;
        foreach (HabBotController bot in _controllers)
        {
            float distance = Vector3.Distance(bot.GetPosition(), position);
            if (distance < distanceMin)
            {
                controller = bot;
                distanceMin = distance;
            }
        }
        return controller;
    }
}
[Serializable]
public struct HabBotPrefab
{
    public HabBot.State _state;
    public GameObject _prefab;
}
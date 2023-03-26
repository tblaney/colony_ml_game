using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingNodeObject : NodeObject
{
    [Header("Inputs:")]
    public int _buildingIndex;
    public int _buildingIndexComplete;
    Building _bldg;

    // cache:
    HabBotController _botController;
    Action BotCallbackFunc;
    bool _building;
    bool _finished = false;

    public override void InitializeNode()
    {
        _bldg = BuildingHandler.Instance.GetBuilding(_buildingIndex);
    }
    public bool UpdatePosition(Vector3Int position)
    {
        return false;
    }
    public void Build(HabBotController controller, Action CallbackFunc)
    {
        if (!_building)
            return;
        
        _botController = controller;
        BotCallbackFunc = CallbackFunc;
        _finished = false;
        InitiateBuild();
    }
    void InitiateBuild()
    {
        _building = true;
        _finished = false;
        Invoke("FinishBuild", _bldg._buildTime);
    }
    public void CancelBuild()
    {
        CancelInvoke();
        _building = false;
        _finished = false;
    }
    void FinishBuild()
    {
        _building = false;
        _finished = true;
        if (BotCallbackFunc != null)
            BotCallbackFunc();
        DestroyNode();
    }
    public override void OnDestroyNode()
    {
        //base.OnDestroyNode();
        if (_finished)
            HabitationHandler.Instance.NewNode(BuildingHandler.Instance.GetBuilding(_buildingIndexComplete).GetNode(this._node._position));
    }
}

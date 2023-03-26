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
    [SerializeField] private MaterialController _controller;
    Building _building;
    // cache:
    HabBotController _botController;
    Action BotCallbackFunc;
    bool _finished = false;

    public override void InitializeNode()
    {
        _building = BuildingHandler.Instance.GetBuilding(_buildingIndex);
    }
    public bool CanPlace()
    {
        return _caster.BlockCast();
    }
    public override void OnDestroyNode()
    {
        //base.OnDestroyNode();
        if (_finished)
            HabitationHandler.Instance.NewNode(BuildingHandler.Instance.GetBuilding(_buildingIndexComplete).GetNodeBuilt(this._node._position));
    }
}

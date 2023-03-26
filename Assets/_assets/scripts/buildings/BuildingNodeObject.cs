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
    [SerializeField] private MaterialController _materialController;
    Building _building;
    // cache:
    HabBotController _botController;
    Action BotCallbackFunc;
    bool _finished = false;
    bool _canPlace = false;

    public override void InitializeNode()
    {
        _building = BuildingHandler.Instance.GetBuilding(_buildingIndex);
        _canPlace = false;
    }
    public void UpdatePosition(Vector3Int position)
    {
        transform.position = position;
        _canPlace = _caster.BlockCast();
        _materialController.ActivateBehaviour(1, _canPlace);
    }
    public bool CanPlace()
    {
        return _canPlace;
    }
    public override void OnDestroyNode()
    {
        //base.OnDestroyNode();
        if (_finished)
            HabitationHandler.Instance.NewNode(BuildingHandler.Instance.GetBuilding(_buildingIndexComplete).GetNodeBuilt(this._node._position));
    }
}

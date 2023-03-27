using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingNodeObject : NodeObject
{
    [Header("Inputs:")]
    public int _buildingIndex;
    [SerializeField] private List<MaterialController> _materialControllers;
    Building _building;
    // cache:
    bool _finished = false;
    bool _canPlace = false;

    public override void InitializeNode()
    {
        _building = BuildingHandler.Instance.GetBuilding(_buildingIndex);
        _canPlace = false;
        HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Craft, new NodeQueueObject(this));
    }
    public void UpdatePosition(Vector3Int position)
    {
        transform.position = position;
        _canPlace = _caster.BlockCast();
        foreach (MaterialController matController in _materialControllers)
        {
            matController.ActivateBehaviour(1, _canPlace);
        }
    }
    public bool CanPlace()
    {
        return _canPlace;
    }
    public override void OnDestroyNode()
    {
        HabitationHandler.Instance.RemoveObjectFromQueue(HabBot.State.Craft, new NodeQueueObject(this));

        //base.OnDestroyNode();
        if (_finished)
            HabitationHandler.Instance.NewNode(BuildingHandler.Instance.GetBuilding(_buildingIndex).GetNodeBuilt(this._node._position));
    }
}

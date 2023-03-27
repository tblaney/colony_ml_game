using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuiltNodeObject : NodeObject
{
    [Header("Inputs:")]
    public int _buildingIndex;
    public BuiltNodeBehaviour _behaviour;
    public InteractableBuiltObject _interactable;

    [Header("Debug:")]
    Building _building;

    // cache:
    
    public override void InitializeNode()
    {
        _building = BuildingHandler.Instance.GetBuilding(_buildingIndex);
        _interactable.Setup(_building);
        if (_behaviour != null)
            _behaviour.Initialize(_building, _node);
    }
    public override void OnDestroyNode()
    {

        //base.OnDestroyNode();
    }
    void Update()
    {
        if (_behaviour != null)
            _behaviour.UpdateBehaviour();
    }
    public Building GetBuilding()
    {
        return _building;
    }
}

[Serializable]
public abstract class BuiltNodeBehaviour : MonoBehaviour
{
    protected Building _building;
    protected Node _node;
    public Bounds _zone;

    public void Initialize(Building building, Node node)
    {
        _building = building;
        _node = node;
        _zone.center = transform.position;
        StartBehaviour();
    }
    public virtual void StartBehaviour(){}
    public virtual void UpdateBehaviour(){}
    public virtual void StopBehaviour(){}

    public Vector3 GetZonePosition()
    {
        return Utils.Tools.GetRandomPositionInBounds(_zone, 30f);
    }
    public bool IsWithinZone(Vector3 position)
    {
        return _zone.Contains(position);
    }

}
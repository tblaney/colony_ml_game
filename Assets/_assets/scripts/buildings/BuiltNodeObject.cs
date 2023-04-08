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
    public Building _building;

    // cache:
    
    public override void InitializeNode()
    {
        _building = BuildingHandler.Instance.GetBuilding(_buildingIndex);
        if (_interactable != null)
            _interactable.Setup(_node, _building);
        if (_behaviour != null)
            _behaviour.Initialize(this);
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
    protected BuiltNodeObject _obj;
    public Bounds _zone;

    public void Initialize(BuiltNodeObject obj)
    {
        _obj = obj;
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuiltNodeObject : NodeObject
{
    [Header("Inputs:")]
    public int _buildingIndex;
    Building _bldg;

    // cache:
    
    public override void InitializeNode()
    {
        _bldg = BuildingHandler.Instance.GetBuilding(_buildingIndex);
    }
    public override void OnDestroyNode()
    {
        //base.OnDestroyNode();
    }
}

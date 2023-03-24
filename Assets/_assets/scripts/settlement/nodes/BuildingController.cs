using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public int _buildingIndex;
    public Building _building;

    void Start()
    {
        _building = BuildingHandler.Instance.GetBuilding(_buildingIndex);
    }
}

public abstract class BuildingControllerState : NodeObject
{
    
}
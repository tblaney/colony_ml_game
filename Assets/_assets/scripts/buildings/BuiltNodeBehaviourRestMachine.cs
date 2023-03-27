using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuiltNodeBehaviourRestMachine : BuiltNodeBehaviour
{
    [SerializeField] private Transform _rotator;
    public override void StartBehaviour()
    {

    }
    public override void UpdateBehaviour()
    {
        Vector3 lightRotation = LightHandler.Instance.GetLightRotation();
        Vector3 rotation = new Vector3(0f, lightRotation.y - 180f, 0f);
        _rotator.eulerAngles = rotation;
    }   
}

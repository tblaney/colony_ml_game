using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuiltNodeBehaviourRestMachine : BuiltNodeBehaviour
{
    [SerializeField] private Transform _rotator;
    [SerializeField] private GameObject _boundsCube;
    public override void StartBehaviour()
    {
        GetComponent<InteractableBuiltObject>().OnHoverFunc = HoverCallback;
        HoverCallback(false);
    }
    void HoverCallback(bool val)
    {
        _boundsCube.SetActive(val);
    }
    public override void UpdateBehaviour()
    {
        Vector3 lightRotation = LightHandler.Instance.GetLightRotation();
        Vector3 rotation = new Vector3(0f, lightRotation.y - 180f, 0f);
        _rotator.eulerAngles = rotation;
    }   
}

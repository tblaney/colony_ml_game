using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHandler : MonoBehaviour, IHandler
{
    public static LightHandler Instance;
    public LightController _controller;
    public void Initialize()
    {
        Instance = this;
    }
    public Vector3 GetLightRotation()
    {
        return _controller.transform.eulerAngles;
    }

}

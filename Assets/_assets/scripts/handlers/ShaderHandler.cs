using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderHandler : MonoBehaviour, IHandler
{
    public static ShaderHandler Instance;
    [Range(0f, 1f)]
    public float _gridOpacity;

    public void Initialize()
    {
        Instance = this;
    }
    void Update()
    {
        Shader.SetGlobalFloat("_GridOpacity", _gridOpacity);
    }
    public void SetGridOpacity(float val)
    {
        _gridOpacity = val;
    }
}

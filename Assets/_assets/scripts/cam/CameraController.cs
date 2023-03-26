using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public float[] zoomRange = new float[] {16f, 26f};
    Camera cam;
    CameraCaster _caster;

    void Awake()
    {
        cam = GetComponent<Camera>();
        _caster = GetComponent<CameraCaster>();
    }
    void Update()
    {
        //if (UIHandler.Instance.IsMouseOverUI())
        //    return;
            
        Zoom();
    }
    void Zoom()
    {
        if (UIHandler.Instance.IsMouseOverUI())
            return;
        
        if (Input.mouseScrollDelta.y != 0f)
        {
            float targetFOV = cam.fieldOfView - Input.mouseScrollDelta.y*16;
            float val = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime*5f);
            if (val > zoomRange[1])
                val = zoomRange[1];
            if (val < zoomRange[0])
                val = zoomRange[0];
            cam.fieldOfView = val;
        }
    }
    public Vector3 GetCenterTerrainPosition()
    {
        return _caster.GetCenterTerrainPosition();
    }
}

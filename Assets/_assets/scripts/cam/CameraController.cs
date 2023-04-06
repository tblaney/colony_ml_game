using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public float[] zoomRange = new float[] {16f, 26f};
    [SerializeField] private CameraTargetController _targetController;
    Camera cam;
    CameraCaster _caster;
    bool _zoomLocked = false;
    float _fov;

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
        if (_zoomLocked)
        {
            float fovNew = Mathf.Lerp(cam.fieldOfView, _fov, Time.deltaTime*4f);
            cam.fieldOfView = fovNew;
            return;
        }
        
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
    public Vector3 GetMouseTerrainPosition()
    {
        return _caster.GetMouseTerrainPosition();
    }
    public void SetTargetFollow(ITarget target)
    {
        _targetController.SetTarget(target);
    }
    public void SetManualControl()
    {
        _targetController.SetManual();
        _zoomLocked = false;
    }
    public void SetMove(Vector3 position)
    {
        _targetController.SetMove(position);
        _zoomLocked = false;
    }
    public void SetFOV(float val)
    {
        _zoomLocked = true;
        //cam.fieldOfView = val;
        _fov = val;
    }
}

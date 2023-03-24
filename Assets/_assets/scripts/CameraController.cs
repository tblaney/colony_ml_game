using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public enum State
    {
        Manual,
        Follow,
    }
    public State state;
    public float movementSpeed = 12f;
    public float[] zoomRange = new float[] {16f, 26f};
    Camera cam;
    Transform target;
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
            
        Vector3 targetPosition = transform.position;
        switch (state)
        {
            case State.Manual:
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                Vector3 moveDirection = (new Vector3(transform.forward.x, 0f, transform.forward.z))*y + transform.right*x;
                moveDirection = moveDirection.normalized;
                targetPosition = transform.position + moveDirection;
                break;
            case State.Follow:
                targetPosition = target.position;
                break;
        }
        float sprintFactor = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
            sprintFactor = 1.8f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*movementSpeed*sprintFactor);

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
    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
        state = State.Follow;
    }
    public void SetManual()
    {
        target = null;
        state = State.Manual;
    }
    public Vector3 GetCenterTerrainPosition()
    {
        return _caster.GetCenterTerrainPosition();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIObjectWorld : MonoBehaviour
{
    Transform _camera;
    void Awake()
    {
        Initialize();
    }
    void Start()
    {
        _camera = CameraHandler.Instance.GetCameraTransform();
    }
    void Update()
    {
        if (_camera == null)
            return;
        // need to rotate towards camera
        Vector3 dir = (_camera.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = lookRotation;
    }
    public virtual void Initialize(){}
}

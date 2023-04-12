using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMimic : MonoBehaviour
{
    public Camera _target;
    Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }
    void Update()
    {
        _cam.orthographicSize = _target.orthographicSize;
    }
}

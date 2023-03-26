using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour, IHandler
{
    public static CameraHandler Instance;
    public CameraController _controller;

    public void Initialize()
    {
        Instance = this;
    }

    public Vector3 GetBuildingPosition()
    {
        // returns mouse position if on terrain
        Vector3 position = default(Vector3);
        if (UIHandler.Instance.IsMouseOverUI())
        {
            position = _controller.GetCenterTerrainPosition();
            return position;
        } else
        {
            position = _controller.GetMouseTerrainPosition();
            if (position != default(Vector3))
                return position;
            
            return _controller.GetCenterTerrainPosition();
        }
    }
}

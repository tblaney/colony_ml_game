using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightController : MonoBehaviour
{
    Light _light;
    [SerializeField] private List<LightPoint> _points;

    void Awake()
    {
        _light = GetComponent<Light>();
    }
    void Update()
    {
        LightPoint[] points = GetPoints();
        LightPoint newPoint = LightPoint.Lerp(points[0], points[1], TimeHandler._timeWorld._dayTime);
        _light.intensity = newPoint._intensity;
        _light.transform.rotation = newPoint._rotation;
    }
    LightPoint[] GetPoints()
    {
        LightPoint p1 = new LightPoint();
        LightPoint p2 = new LightPoint();
        bool foundp1 = false;
        bool foundp2 = false;
        float hour = TimeHandler._timeWorld._dayTime;
        foreach (LightPoint point in _points)
        {
            if (hour >= point._hour)
            {
                p1 = point;
                foundp1 = true;
            }
        }
        foreach (LightPoint point in _points)
        {
            if (hour < point._hour)
            {
                p2 = point;
                foundp2 = true;
                break;
            }
        }
        if (!foundp2)
        {
            float hourMin = 10000f;
            foreach (LightPoint point in _points)
            {
                if (point._hour < hourMin)
                {
                    hourMin = point._hour;
                    p2 = point;
                }
            }
        }
        return new LightPoint[] {p1, p2};
    }
}

[Serializable]
public struct LightPoint
{
    public string _name;
    public float _hour;
    public float _intensity;
    public Quaternion _rotation;
    public static LightPoint Lerp(LightPoint p1, LightPoint p2, float hour)
    {
        float difference = p2._hour - p1._hour;
        float val = (hour - p1._hour)/difference;
        LightPoint point = new LightPoint();
        point._intensity = Mathf.Lerp(p1._intensity, p2._intensity, val);
        point._rotation = Quaternion.Lerp(p1._rotation, p2._rotation, val);
        return point;
    }
}

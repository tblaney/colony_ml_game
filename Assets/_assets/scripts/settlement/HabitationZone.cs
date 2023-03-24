using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HabitationZone 
{
    public enum Type
    {
        Rest,
        Store,
        Farm,
    }
    public Type _type;
    public Bounds _bounds;

    public void Initialize(Type type, Bounds bounds)
    {
        _type = type;
        _bounds = bounds;
    }
    public Vector3 GetRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(_bounds.min.x, _bounds.max.x), _bounds.center.y, UnityEngine.Random.Range(_bounds.min.z, _bounds.max.z));
    }
}

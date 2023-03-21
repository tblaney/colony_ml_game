using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class HabitationZone 
{
    public enum Type
    {
        Rest,
    }
    public Type _type;
    public Bounds _bounds;
    public bool _local;

    public virtual void Initialize(Type type, Bounds bounds)
    {
        _type = type;
        _bounds = bounds;
    }
    public Vector3 GetRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(_bounds.min.x, _bounds.max.x), _bounds.center.y, UnityEngine.Random.Range(_bounds.min.z, _bounds.max.z));
    }
}

public class HabitationZoneGlobal : HabitationZone
{

}

public class HabitationZoneLocal : HabitationZone
{
    // belonging to a specific bot
    public HabBot _bot;

    public override void Initialize(Type type, Bounds bounds)
    {
        base.Initialize(type, bounds);
        _local = true;
    }
}
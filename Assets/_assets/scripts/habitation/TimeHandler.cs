using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeHandler : MonoBehaviour, IHandler
{
    public static TimeHandler Instance;

    public static event EventHandler OnMidnight;
    public float _timeScale = 1f;
    public bool _locked;

    public void Initialize()
    {
        Instance = this;
    }

    void Update()
    {
        Debug.Log("Time Handler Scale: " + Time.timeScale);
        if (_locked)
            return;
        
        if (!Mathf.Approximately(Time.timeScale, _timeScale))
        {
            Time.timeScale = _timeScale;
        }
    }

    public void SetTimeScale(float val)
    {
        _timeScale = val;
    }

    public void Lock(bool locked = true)
    {
        _locked = locked;
    }


}
[Serializable]
public class TimeWorld
{
    public float _time;
    public int _day;
}

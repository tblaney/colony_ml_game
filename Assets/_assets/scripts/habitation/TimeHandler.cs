using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeHandler : MonoBehaviour, IHandler
{
    public static TimeHandler Instance;
    public static TimeWorld _timeWorld;
    public float _timeScale = 1f;
    public int _timeStart = 2;
    public bool _locked;

    public void Initialize()
    {
        Instance = this;
        _timeWorld = new TimeWorld(){};
        _timeWorld._dayMinutes = 6;
        _timeWorld._timer = _timeStart*60f;
        _timeWorld.Initialize();
    }

    void Update()
    {
        _timeWorld.UpdateTime();

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
    public void SetTimeScaleLock(float val)
    {
        Time.timeScale = val;
        Lock(true);
    }
    public void Lock(bool locked = true)
    {
        _locked = locked;
    }


}
[Serializable]
public class TimeWorld
{
    [Header("Inputs:")]
    [Tooltip("Time it takes for a day to pass irl")]
    public int _dayMinutes;
    public event EventHandler OnMidnight;

    [Header("Debug:")]
    public float _dayTime;
    public string _date;
    public int _day;

    public float _timer;
    
    public void Initialize(TimeWorld timeIn = null)
    {
        if (timeIn == null)
        {
            _day = 1;
            _dayTime = 0f;
        } else
        {
            this._dayMinutes = timeIn._dayMinutes;
            this._dayTime = timeIn._dayTime;
            this._day = timeIn._day;
            this._timer = timeIn._timer;
        }
    }
    public void UpdateTime()
    {
        _timer += Time.deltaTime;
        if (_timer > (_dayMinutes*60f))
        {
            _timer = 0f;
            _day ++;
            OnMidnight?.Invoke(null, EventArgs.Empty);
        }
        _dayTime = (24*(_timer/(_dayMinutes*60f)));
        UpdateDate();
    }
    void UpdateDate()
    {
        var ts = TimeSpan.FromSeconds(_dayTime*60f*60f);
        string t = string.Format("{0:00}:{1:00}", ts.Hours, ts.Minutes);
        _date = t;
    }
}

[Serializable]
public class TimeAction
{
    public TimeWorld _time;
    public Action OnTimeFunc;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIVitality : UIObject
{
    Vitality _vitality;
    bool _subscribed = false;
    public override void Initialize()
    {

    }
    public void Setup(Vitality vitality)
    {
        Unsubscribe();
        _vitality = vitality;
        Subscribe();
        VitalityRefresh(null, EventArgs.Empty);
    }
    void Subscribe()
    {
        if (_subscribed)
            return;
        if (_vitality == null)
            return;
        _vitality.OnValueChange += VitalityRefresh;
        _subscribed = true;
    }
    void Unsubscribe()
    {
        if (_vitality == null)
            return;
        _vitality.OnValueChange -= VitalityRefresh;
        _subscribed = false;
    }
    void OnEnable()
    {
        Subscribe();
        Refresh();
    }
    void OnDisable()
    {
        Unsubscribe();
    }
    void VitalityRefresh(object sender, EventArgs e)
    {
        Refresh(0.25f);
    }

    void Refresh(float time = 0f)
    {
        if (_vitality == null)
            return;
            
        float valNormalized = _vitality.GetVitalityNormalized();
        Vector3 scale = new Vector3(1f, valNormalized, 1f);
        _controller.SetScale(scale, 1, time);
        _controller.SetText(_vitality._val.ToString(), 2);
    }
}

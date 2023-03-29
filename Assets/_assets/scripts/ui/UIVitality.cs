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
        _vitality = vitality;
        Subscribe();
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
    void OnEnable()
    {
        Subscribe();
    }
    void OnDisable()
    {
        if (_vitality == null)
            return;
        _vitality.OnValueChange -= VitalityRefresh;
        _subscribed = false;
    }
    void VitalityRefresh(object sender, EventArgs e)
    {
        float valNormalized = _vitality.GetVitalityNormalized();
        //Debug.Log("Vitality Refresh: " + valNormalized);
        Vector3 scale = new Vector3(1f, valNormalized, 1f);
        _controller.SetScale(scale, 1, 0.25f);
        _controller.SetText(_vitality._val.ToString(), 2);
    }
}

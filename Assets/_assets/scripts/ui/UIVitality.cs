using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIVitality : UIObject
{
    Vitality _vitality;
    public override void Initialize()
    {

    }
    public void Setup(Vitality vitality)
    {
        _vitality = vitality;
        _vitality.OnValueChange += VitalityRefresh;
    }
    void OnDisable()
    {
        _vitality.OnValueChange -= VitalityRefresh;
    }
    void VitalityRefresh(object sender, EventArgs e)
    {
        float valNormalized = _vitality.GetVitalityNormalized();
        Vector3 scale = new Vector3(1f, valNormalized, 1f);
        _controller.SetScale(scale, 1, 0.25f);
        _controller.SetText(_vitality._val.ToString(), 2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ITooltipObject 
{
    TooltipInfo GetInfo();
    void ActivateHighlight(bool active = true);
}

[Serializable]
public class TooltipInfo
{
    public string _name;
    public string _typeText;
    public string _description;
}

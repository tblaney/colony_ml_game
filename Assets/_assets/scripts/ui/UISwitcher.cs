using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] private List<UISwitcherObject> _switchers;
    public int _indexCurrent;

    void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        foreach (UISwitcherObject switcher in _switchers)
        {
            switcher._uiButton.OnPointerClickFunc = () =>
            {
                Switch(switcher._index);
            };
        }
    }
    void Switch(int index)
    {
        if (_indexCurrent != 0)
        {
            // turn off
            UISwitcherObject obj = GetSwitcherObject(_indexCurrent);
            obj._uiObject.ActivateUI(false);
            if (_indexCurrent == index)
            {
                _indexCurrent = 0;
                return;
            }
        } 
        UISwitcherObject newObj = GetSwitcherObject(index);
        newObj._uiObject.ActivateUI(true);
        _indexCurrent = index;
    }
    UISwitcherObject GetSwitcherObject(int index)
    {
        foreach (UISwitcherObject switcher in _switchers)
        {
            if (switcher._index == index)
                return switcher;
    
        }
        return default(UISwitcherObject);
    }
}
[Serializable]
public struct UISwitcherObject
{
    public int _index;
    public UIObject _uiObject;
    public UIButton _uiButton;
}
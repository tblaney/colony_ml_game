using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIHandler : MonoBehaviour, IHandler
{
    public static UIHandler Instance;
    public UIHabitation _uiHabitation;

    public void Initialize()
    {
        Instance = this;
    }
    public void InitializeHabitation(Habitation habitation)
    {
        _uiHabitation.Setup(habitation);
    }
    public bool IsMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
[Serializable]
public class UIButtonInput
{
    public string _name;
    public int _index;
    public UIButton _button;
}

public interface IHandler
{
    void Initialize();
}
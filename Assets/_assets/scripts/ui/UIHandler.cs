using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIHandler : MonoBehaviour, IHandler
{
    public static UIHandler Instance;
    public UIHabitation _uiHabitation;
    public UITooltip _uiTooltip;
    public UINotification _uiNoficiation;
    public static event EventHandler OnStateViewToggle;

    List<GameObject> _objs;

    public void Initialize()
    {
        Instance = this;
        _objs = new List<GameObject>();
    }
    void Update()
    {
        MouseUpdate();
    }
    void MouseUpdate()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        _objs.Clear();
        foreach (RaycastResult result in results)
        {
            _objs.Add(result.gameObject);
        }
    }
    public void InitializeHabitation(Habitation habitation)
    {
        _uiHabitation.Setup(habitation);
    }
    public bool IsMouseOverUI()
    {
        return _objs.Count > 1;
    }
    public List<GameObject> GetGameObjectsUnderMouse()
    {
        return _objs;
    }
    public void ActivateTooltip(bool active, string text = "")
    {
        _uiTooltip.ActivateTooltip(active, text);
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
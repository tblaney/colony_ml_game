using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIHandler : MonoBehaviour, IHandler
{
    public static UIHandler Instance;
    public UIHabitation _uiHabitation;
    public static event EventHandler OnStateViewToggle;

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
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        Debug.Log("Mouse Over: " + results.Count);
        if (results.Count > 0)
        {
            //Debug.Log("Mouse Over Object: " + results[0].gameObject);
        }
        return results.Count > 1;
        /*
        PointerEventData eventDataCurrent = new PointerEventData(EventSystem.current);
        Debug.Log("Mouse Over: " + eventDataCurrent.hovered.Count);
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Debug.Log ("Mouse Over:" + EventSystem.current.currentSelectedGameObject.gameObject.name);
            return true;
        }
        return false;
        */
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
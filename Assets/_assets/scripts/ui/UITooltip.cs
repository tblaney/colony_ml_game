using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UITooltip : MonoBehaviour
{
    [SerializeField] private List<UIController> _controllers;
    UIController _controller;
    RectTransform _rect;
    bool _active = false;

    void Awake()
    {   
        _rect = GetComponent<RectTransform>();
    }
    void Start()
    {
        ActivateTooltip(false);
        foreach (UIController controller in _controllers)
        {
            controller.ActivateBehaviour("activate", false);
        }
    }
    void Update()
    {
        if (!_active)
            return;

        _rect.anchoredPosition = Input.mousePosition;
    }   
    public void ActivateTooltip(bool active, string text = "")
    {
        _active = active;
        UIController controller = GetController(Input.mousePosition);
        if (_active)
        {
            controller.ActivateBehaviour("activate", true);
            controller.SetText(text, "tooltip");
        } else
        {
            if (_controller != null)
                _controller.ActivateBehaviour("activate", false);
        }
        _controller = controller;
    }
    UIController GetController(Vector2 mousePos)
    {
        Vector2 screen = new Vector2(Screen.width, Screen.height);
        if (mousePos.x > screen.x/2f)
        {
            //q1, q2
            if (mousePos.y > screen.y/2f)
            {
                //q1
                return _controllers[0];
            } else
            {
                //q2
                return _controllers[1];
            }
        } else
        {
            //q3, q4
            if (mousePos.y > screen.y/2f)
            {
                //q4
                return _controllers[3];
            } else
            {
                //q3
                return _controllers[2];
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UITooltip : MonoBehaviour
{
    [SerializeField] private UIController _controller;

    RectTransform _rect;
    bool _active = false;

    void Awake()
    {   
        _rect = GetComponent<RectTransform>();
    }
    void Start()
    {
        ActivateTooltip(false);
    }
    void Update()
    {
        if (!_active)
            return;

        _rect.anchoredPosition = Input.mousePosition;
    }   
    public void ActivateTooltip(bool active, TooltipInfo tooltipInfo = null)
    {
        _active = active;
        if (_active)
        {
            _controller.ActivateBehaviour("activate", true);
            
            _controller.SetText(tooltipInfo._name, "name");
            _controller.SetText(tooltipInfo._typeText, "type");
            _controller.SetText(tooltipInfo._description, "description");
        } else
        {
            _controller.ActivateBehaviour("activate", false);
            _controller.SetText("", "name");
            _controller.SetText("", "type");
            _controller.SetText("", "description");
        }
    }
}


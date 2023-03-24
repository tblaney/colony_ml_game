using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIHabBotWorld : MonoBehaviour
{
    UIController _controller;
    HabBot _bot;
    public void Initialize(HabBot bot)
    {
        _bot = bot;
        _bot.OnStateChange += Bot_StateChange;
        UIHandler.OnStateViewToggle += UI_StateView;

        _controller = GetComponent<UIController>();
        
        Refresh();
        Activate(false);
    }
    void OnDisable()
    {
        if (_bot != null)
            _bot.OnStateChange -= Bot_StateChange;
        
        UIHandler.OnStateViewToggle -= UI_StateView;
    }
    void Bot_StateChange(object sender, EventArgs e)
    {
        Refresh();
    }
    void UI_StateView(object sender, EventArgs e)
    {
        
    }
    public void Activate(bool active = true)
    {
        this.gameObject.SetActive(active);
    }
    public void Refresh()
    {
        Sprite sprite = HabitationHandler.Instance.GetStateSprite(_bot._state);
        if (sprite == null)
            return;
            
        _controller.SetImage(sprite, 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIExit : UIObject
{
    // saves and exits
    public UIButton _buttonExit;
    public bool _active;
    public override void Initialize()
    {
        _buttonExit.OnPointerClickFunc = SaveAndExit;
    }
    void Start()
    {
        Activate(false);
    }
    public override void Activate(bool active)
    {
        Debug.Log("UI Exit Activate: " + active); 
        if (active)
        {
            GameHandler.Instance.Pause();
            _controller.ActivateBehaviour("activate object", true);
            _controller.ActivateBehaviour("activate", true);
        } else
        {
            if (GameHandler._paused)
                GameHandler.Instance.Resume();
            _controller.ActivateBehaviour("activate object", false);
        }
        _active = active;
    }
    void SaveAndExit()
    {
        GameHandler.Instance.Save();

        Invoke("CloseApplication", 1f);
    }
    void CloseApplication()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIEnemy : UIObject
{
    [SerializeField] private UIVitality _uiVitality;
    [SerializeField] private UIButton _buttonClose;

    IEnemy _enemy;
    bool _active;

    public override void Initialize()
    {
        _buttonClose.OnPointerClickFunc = () =>
        {
            ActivateUI(false);
        };
    }
    void Start()
    {
        ActivateUI(false);
    }
    public void Setup(IEnemy enemy)
    {
        _enemy = enemy;
        _uiVitality.Setup(_enemy.GetVitality());
        ActivateUI(true);
    }
    public override void Activate(bool active = true)
    {
        if (active)
        {
            UserHandler._target = _enemy.GetTarget();
            UserHandler.Instance.SetUserState(UserController.State.Following);

            _active = active;
            Refresh();
            _controller.ActivateBehaviour("object activate", true);
            _controller.ActivateBehaviour("fade", true);
        } else
        {
            UserHandler._target = null;
            UserHandler.Instance.SetUserState(UserController.State.Viewing);

            _active = false;
            _controller.ActivateBehaviour("fade", false);
        }
    }
    void NullClick(object sender, EventArgs e)
    {
        if (_active)
            ActivateUI(false);
    }
    void Refresh()
    {
        _controller.SetText(_enemy.GetStrings()[0], "name");
        _controller.SetText(_enemy.GetStrings()[1], "description");
    }
}

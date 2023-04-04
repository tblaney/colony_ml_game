using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UINode : UIObject
{
    [SerializeField] private UIVitality _uiVitality;
    [SerializeField] private UIButton _buttonQueue;
    [SerializeField] private UIButton _buttonClose;

    Node _node;
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
    public void Setup(Node node, Building building = null)
    {
        _node = node;
        _uiVitality.Setup(_node._health);
        ActivateUI(true);
    }
    public override void Activate(bool active = true)
    {
        if (active)
        {
            UserHandler._target = _node;
            UserHandler.Instance.SetUserState(UserController.State.Following);
            //UIHandler.OnNullClick += NullClick;
            _active = active;
            Refresh();
            _controller.ActivateBehaviour("object activate", true);
            _controller.ActivateBehaviour("fade", true);
        } else
        {
            UserHandler._target = null;
            UserHandler.Instance.SetUserState(UserController.State.Viewing);
            //UIHandler.OnNullClick -= NullClick;
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
        if (_node.IsQueued())
        {
            _controller.ActivateBehaviour("queue color on");
        } else
        {
            _controller.ActivateBehaviour("queue color normal");
        }
        PrefabInput prefab = PrefabHandler.Instance.GetPrefabInput(_node._prefab);
        _controller.SetText(prefab._name, "name");
        _controller.SetText(Node.GetDescription(_node), "description");
        _buttonQueue.OnPointerClickFunc = () =>
        {
            if (_node.IsQueued())
            {
                // dequeue
                HabitationHandler.Instance.RemoveObjectFromQueue(_node.GetState(), _node);
            } else
            {
                HabitationHandler.Instance.AddObjectToQueue(_node.GetState(), _node);
            }
            Refresh();
        };
    }
}

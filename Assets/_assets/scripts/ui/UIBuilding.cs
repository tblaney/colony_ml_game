using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBuilding : UIObject
{
    // sets necessary info for this built object, and displays options
    public int _indexBuilding;
    [SerializeField] public UIButton _buttonClose;
    [SerializeField] private UIButton _buttonDestroyQueue;
    protected Building _building;
    protected Node _node;
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
    public virtual void SetupBuilding(){}
    public void Setup(Node node, Building building)
    {
        _node = node;
        _building = building;
        SetupBuilding();
        ActivateUI(true);
    }
    public override void Activate(bool active = true)
    {
        if (active)
        {
            UserHandler._target = _node;
            UserHandler.Instance.SetUserState(UserController.State.Following);
            _active = active;
            RefreshBuilding();
            _controller.ActivateBehaviour("object activate", true);
            _controller.ActivateBehaviour("fade", true);
        } else
        {
            UserHandler._target = null;
            UserHandler.Instance.SetUserState(UserController.State.Viewing);
            _active = false;
            _controller.ActivateBehaviour("fade", false);
        }
        ActivateBuilding(active);
    }
    public virtual void ActivateBuilding(bool val)
    {

    }
    void RefreshBuilding()
    {
        if (_node.IsQueued())
        {
            _controller.ActivateBehaviour("queue color on");
        } else
        {
            _controller.ActivateBehaviour("queue color normal");
        }
        _controller.SetText(_building._name, "name");
        _controller.SetText(_building._description, "description");
        _buttonDestroyQueue.OnPointerClickFunc = () =>
        {
            if (_node.IsQueued())
            {
                // dequeue
                HabitationHandler.Instance.RemoveObjectFromQueue(HabBot.State.Destroy, _node);
            } else
            {
                HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Destroy, _node);
            }
            RefreshBuilding();
        };
        Refresh();
    }
    public virtual void Refresh()
    {

    }
    public override void DestroyUI()
    {
        base.DestroyUI();
    }
}

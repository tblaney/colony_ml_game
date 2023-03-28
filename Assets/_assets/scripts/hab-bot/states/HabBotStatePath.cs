using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStatePath : HabBotState
{
    Notification _notification;
    public override void StartState()
    {
        _notification = new Notification(Notification.Type.State, "Pathing State: " + _controller.GetBot()._name);
        NotificationHandler.Instance.NewNotification(_notification);
        //UserHandler.Instance.SetUserState(UserController.State.Pathing);
    }

    public override void StopState()
    {
        if (_notification != null)
            NotificationHandler.Instance.ClearNotification(_notification);
        base.StopState();
    }

    public override void UpdateState()
    {

    }
}   

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationHandler : MonoBehaviour, IHandler
{
    public static NotificationHandler Instance;
    public static event EventHandler<NotificationEventArgs> OnNotification;
    public static event EventHandler<NotificationEventArgs> OnNotificationClear;
    public class NotificationEventArgs
    {
        public Notification _notification;
    };

    public void Initialize()
    {
        Instance = this;
    }
    public void NewNotification(Notification notification)
    {
        NotificationEventArgs args = new NotificationEventArgs(){_notification = notification};
        OnNotification?.Invoke(this, args);
    }
    public void ClearNotification(Notification notification)
    {
        NotificationEventArgs args = new NotificationEventArgs(){_notification = notification};
        OnNotificationClear?.Invoke(this, args);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //NewNotification(new Notification() {_notification = "test notification please", _type = Notification.Type.General});
            //NewNotification(new Notification() {_notification = "test notification please", _type = Notification.Type.State});
        }
    }

}
[Serializable]
public class Notification
{
    public enum Type
    {
        General, //right hand side
        State, // top
        Item, // top left item
    }
    public Type _type;
    public string _notification;
    public NotificationEvent _event;
    public Action OnClickFunc;

    public Notification(Type type, string notification)
    {
        _type = type;
        _notification = notification;
    }
}

[Serializable]
public class NotificationEvent
{
    public enum Type
    {
        Threat,
        Injury,
        Emotional,
    }
    public Vector3Int _position;
}

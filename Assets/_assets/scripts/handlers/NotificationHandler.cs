using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NotificationHandler : MonoBehaviour, IHandler
{
    public static NotificationHandler Instance;
    public static event EventHandler<NotificationEventArgs> OnNotification;
    public static event EventHandler<NotificationEventArgs> OnNotificationClear;
    public class NotificationEventArgs{public Notification _notification;};

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
            NewNotification(new Notification() {_name = "test", _notification = "test notification please"});
    }

}
[Serializable]
public class Notification
{
    public string _name;
    public int _index;
    public string _notification;
    public NotificationEvent _event;
    public Action OnClickFunc;
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

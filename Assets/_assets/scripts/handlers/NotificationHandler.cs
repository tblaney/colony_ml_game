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
    public Notification NewNotification(Notification.Type type, string text, Action OnClickFunc = null)
    {
        Notification notification = new Notification(type, text);
        if (OnClickFunc != null)
            notification.SetClickAction(OnClickFunc);

        NotificationEventArgs args = new NotificationEventArgs(){_notification = notification};
        OnNotification?.Invoke(this, args);

        return notification;
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
            NewNotification(new Notification(Notification.Type.General, "test notification please"){});
            //NewNotification(new Notification(Notification.Type.State, "test notification please"){});
        }
    }

}
[Serializable]
public class Notification
{
    public enum Type
    {
        General, //right hand side
        Top, // top
        Item, // top left item
    }
    public Type _type;
    public string _notification;
    public Action OnClickFunc;
    public int _importance = 0;

    public Notification(Type type, string notification)
    {
        _type = type;
        _notification = notification;
    }
    public void SetClickAction(Action action)
    {
        this.OnClickFunc = action;
    }
}
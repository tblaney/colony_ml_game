using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotification : UIObject
{
    public Notification.Type _type;
    [SerializeField] private Transform _container;
    [SerializeField] private string _prefabNotificationName;
    [SerializeField] private List<Color> _colors = new List<Color>(){Color.white};

    Dictionary<Notification, UIButton> _dic;
    public override void Initialize()
    {
        _dic = new Dictionary<Notification, UIButton>();
    }
    void OnEnable()
    {
        NotificationHandler.OnNotification += NewNotification;
        NotificationHandler.OnNotificationClear += ClearNotification;
    }
    void OnDisable()
    {
        NotificationHandler.OnNotification -= NewNotification;
        NotificationHandler.OnNotificationClear -= ClearNotification;
    }
    void NewNotification(object sender, NotificationHandler.NotificationEventArgs e)
    {
        if (e._notification._type != _type)
            return;
        
        GameObject prefab = PrefabHandler.Instance.GetPrefab(_prefabNotificationName);
        GameObject obj = Instantiate(prefab, _container.transform);
        UIButton button = obj.GetComponent<UIButton>();
        UIController controller = button.GetController();
        controller.SetText(e._notification._notification, "notification");
        //controller.SetTextColor(_colors[e._notification._importance], "notification");
        button.OnPointerClickFunc = e._notification.OnClickFunc;
        _dic.Add(e._notification, button);
    }
    public void ClearNotification(object sender, NotificationHandler.NotificationEventArgs e)
    {
        if (e._notification._type != _type)
            return;

        Notification notification = e._notification;
        UIButton button;
        if (_dic.TryGetValue(notification, out button))
        {
            Destroy(button.gameObject);
            _dic.Remove(notification);
        }
    }
}


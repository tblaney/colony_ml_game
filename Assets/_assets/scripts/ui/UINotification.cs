using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotification : UIObject
{
    [SerializeField] private Transform _container;
    [SerializeField] private string _prefabNotificationName;

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
        GameObject prefab = PrefabHandler.Instance.GetPrefab(_prefabNotificationName);
        GameObject obj = Instantiate(prefab, _container.transform);
        UIButton button = obj.GetComponent<UIButton>();
        button.OnPointerClickFunc = e._notification.OnClickFunc;
        _dic.Add(e._notification, button);
    }
    public void ClearNotification(object sender, NotificationHandler.NotificationEventArgs e)
    {
        Notification notification = e._notification;
        UIButton button;
        if (_dic.TryGetValue(notification, out button))
        {
            Destroy(button.gameObject);
            _dic.Remove(notification);
        }
    }
}

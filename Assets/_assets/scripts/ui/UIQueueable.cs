using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIQueueable : UIObject
{
    Action<Queueable> ButtonClickCallback;
    public UIButton _buttonQueueable;
    HabitationQueue _queue;
    public override void Initialize()
    {

    }
    public void Setup(HabitationQueue queue, Action<Queueable> ButtonClickCallback)
    {
        this.ButtonClickCallback = ButtonClickCallback;
        this._queue = queue;
        Refresh();
    }
    void Refresh()
    {
        Clear();
        foreach (Queueable queueable in _queue._queueables)
        {
            UIButton button = Instantiate(_buttonQueueable, _buttonQueueable.transform.parent);
            UIController controller = button.GetComponent<UIController>();
            button.OnPointerClickFunc = () => {ButtonClickCallback(queueable);};
            switch (queueable._queueableType)
            {
                case Queueable.Type.Node:
                    Node node = queueable as Node;
                    PrefabInput input = PrefabHandler.Instance.GetPrefabInput(node._prefab);
                    controller.SetText(input._name, "name");
                    break;
                case Queueable.Type.Machine:
                    MachineQueuable machineQueuable = queueable as MachineQueuable;
                    controller.SetText(machineQueuable._item._name, "name");
                    break;
                case Queueable.Type.Craft:
                    Item item = queueable as Item;
                    controller.SetText(item._name, "name");
                    break;
            }
        }
    }
    void Clear()
    {
        foreach (Transform child in _buttonQueueable.transform.parent)
        {
            if (child.gameObject == _buttonQueueable.gameObject)
                continue;
            Destroy(child.gameObject);
        }
    }
}

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
        queue.OnQueueChange += Queue_OnQueueChange;
    }
    void OnDisable()
    {
        _queue.OnQueueChange -= Queue_OnQueueChange;
    }
    void Queue_OnQueueChange(object sender, EventArgs e)
    {
        Refresh();
    }
    void Refresh()
    {
        Clear();
        foreach (Queueable queueable in _queue._queueables)
        {
            UIButton button = Instantiate(_buttonQueueable, _buttonQueueable.transform.parent);
            button.Activate(true);
            UIController controller = button.GetComponent<UIController>();
            controller.FormList();
            button.OnPointerClickFunc = () => {ButtonClickCallback(queueable);};
            switch (queueable._state)
            {
                case HabBot.State.CollectMinerals:
                    Node node = queueable as Node;
                    PrefabInput input = PrefabHandler.Instance.GetPrefabInput(node._prefab);
                    controller.SetText(input._name, "name");
                    break;
                case HabBot.State.Machine:
                    MachineQueuable machineQueuable = queueable as MachineQueuable;
                    controller.SetText(machineQueuable._item._name, "name");
                    break;
                case HabBot.State.Craft:
                    ItemInput item = queueable as ItemInput;
                    controller.SetText(item._name, "name");
                    break;
                case HabBot.State.Haul:
                    HaulQueueable haulQueueable = queueable as HaulQueueable;
                    controller.SetText(haulQueueable._item._name, "name");
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

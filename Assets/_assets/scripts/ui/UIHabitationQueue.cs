using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHabitationQueue : UIObject
{
    [Header("Inputs:")]
    public string _prefabNameQueue;
    public GameObject _buttonContainer;
    public float _controllerSpacing = 40f;
    public UIButton _buttonMain;

    [Space(10)]
    List<UIButton> _buttons;
    bool _active;


    public override void Initialize()
    {
        _buttons = new List<UIButton>();
    }
    public override void Activate(bool active = true)
    {
        _active = active;
        if (active)
        {
            Refresh();
        }
    }
    void Refresh()
    {
        List<HabitationQueue> queues = HabitationHandler.Instance.GetQueues();
        ClearAll();
        int y = 0;
        foreach (HabitationQueue queue in queues)
        {
            GameObject prefab = PrefabHandler.Instance.GetPrefab(_prefabNameQueue);
            GameObject obj = Instantiate(prefab, _buttonContainer.transform);
            UIButton button = obj.GetComponent<UIButton>();
            UIQueueable uiQueueable = obj.GetComponent<UIQueueable>();
            uiQueueable.Setup(queue, ButtonClickCallback);
            UIController controller = button.GetController();
            controller.SetText(queue._state.ToString(), "name");
            _buttons.Add(button);
            y++;
        }
        // need to refresh size of contents
        float height = y*_controllerSpacing;
        _controller.SetSize(new Vector2(300f, height + 20f), 10);
    }
    void ButtonClickCallback(Queueable queueable)
    {
        switch (queueable._state)
        {
            case HabBot.State.CollectMinerals:
                Node node = queueable as Node;
                NodeObject nodeObject = HabitationHandler.Instance.GetNodeObject(node);
                Interactable interactable = nodeObject.GetComponent<Interactable>();
                if (interactable != null)
                    interactable.Interact();
                break;
            case HabBot.State.Machine:
                MachineQueuable machineQueuable = queueable as MachineQueuable;
                UIHandler.Instance.ActivateHabBotFocus(machineQueuable._bot);
                break;
        }
        _buttonMain.OnPointerClickFunc();
    }
    void ClearAll()
    {
        foreach (UIButton button in _buttons)
        {
            Destroy(button.gameObject);
        }
        _buttons.Clear();
    }
}

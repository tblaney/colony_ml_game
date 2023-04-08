using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIHabBotAddons : UIObject
{
    public UIButton _buttonAdd;
    public UIButton _buttonAddOption;
    public List<int> _itemAddonIndices;

    bool _buttonActive = false;
    HabBot _bot;

    public override void Initialize()
    {

    }
    public void Setup(HabBot bot)
    {
        _bot = bot;
        //RefreshAddonOptions();
        _buttonAdd.ActivateController(2, false);
    }
    public void RefreshAddonOptions()
    {
        // check if we have any addons to add, if we do, add it to a queue
        List<Item> itemsToAdd = new List<Item>();
        foreach (int j in _itemAddonIndices)
        {
            Item item = HabitationHandler.Instance.GetItem(j);
            if (item != null && item._amount > 0)
            {
                HabBotAddon.Type addonType = (HabBotAddon.Type)(j - 2);
                Debug.Log("Refresh Addon Options: " + addonType);
                if (_bot.ContainsAddon(addonType))
                    continue;
                
                if (HabitationHandler.Instance.IsAddonQueued(item.GetItemInput(), _bot))
                    continue;
                // can add this item
                itemsToAdd.Add(item);
            }
        }
        if (itemsToAdd.Count > 0)
        {
            _buttonAdd.Activate(true);
            _buttonAdd.transform.SetSiblingIndex(_buttonAdd.transform.parent.childCount - 1);
            // refresh buttons in tooltip container
            foreach (Transform child in _buttonAddOption.transform.parent)
            {
                if (child.gameObject == _buttonAddOption.gameObject)
                    continue;
                Destroy(child.gameObject);
            }
            Vector2 rect = _buttonAddOption.GetComponent<RectTransform>().sizeDelta;
            int i = 0;
            foreach (Item item in itemsToAdd)
            {
                UIButton button = Instantiate(_buttonAddOption, _buttonAddOption.transform.parent);
                button.Activate(true);
                UIController controller = button.GetComponent<UIController>();
                controller.FormList();
                HabBotAddon.Type addonType = (HabBotAddon.Type)(item._index - 2);
                controller.SetText(addonType.ToString(), "name");
                button.OnPointerClickFunc = () =>
                {
                    // add to queue
                    MachineQueuable queuable = new MachineQueuable(new ItemInput(){_name = item._name, _index = item._index, _amount = 1}, _bot);
                    HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Machine, queuable);
                    _buttonActive = true;
                    _buttonAdd.OnPointerClickFunc();
                    RefreshAddonOptions();
                };
                i++;
            }
            _buttonAddOption.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.x, rect.y*i);
            _buttonAdd.OnPointerClickFunc = () =>
            {
                // show tooltip
                bool val = !_buttonActive;
                if (val)
                    _buttonAdd.ActivateController(1, val);
                _buttonAdd.ActivateController(2, val);
                _buttonActive = val;
            };  
            
        } else
        {
            _buttonAdd.Activate(false);
        }
    }
}

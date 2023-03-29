using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIBuildingCraft : UIBuilding
{
    [SerializeField] private UIButton _buttonItemDefault;
    [SerializeField] private UIButton _buttonCraft;
    [SerializeField] private UIButton _buttonAmountUp;
    [SerializeField] private UIButton _buttonAmountDown;

    ItemInput _itemSelected;
    UIButton _buttonCache;

    public override void SetupBuilding()
    {
        _buttonAmountDown.OnPointerClickFunc = () =>
        {
            if (_itemSelected != default(ItemInput))
            {
                _itemSelected._amount--;
                if (_itemSelected._amount < 1)
                    _itemSelected._amount = 1;
                
                RefreshItemCraft();
            }
        };
        _buttonAmountUp.OnPointerClickFunc = () =>
        {
            if (_itemSelected != default(ItemInput))
            {
                _itemSelected._amount++;
                if (_itemSelected._amount > 100)
                    _itemSelected._amount = 100;
                
                RefreshItemCraft();
            }
        };
        _buttonCraft.OnPointerClickFunc = () => {CraftCallback();};
    }
    public override void ActivateBuilding(bool val)
    {
        if (!val)
        {
            _controller.ActivateBehaviour(101, false);
        }
    }
    public override void Refresh()
    {
        RefreshItems();
    }
    void RefreshItems()
    {
        Vector2 rect = _buttonItemDefault.GetComponent<RectTransform>().sizeDelta;
        int i = 0;
        foreach (ItemInput item in _building._itemOutputs)
        {
            UIButton button = Instantiate(_buttonItemDefault, _buttonItemDefault.transform.parent);
            button.gameObject.SetActive(true);
            UIController controller = button.GetComponent<UIController>();
            button.OnPointerClickFunc = () =>
            {
                ButtonCallback(button, item);
            };
            controller.FormList();
            controller.SetText(item._name, "name");
            controller.SetText(item._amount.ToString(), "amount");
            i++;
        }
        _buttonItemDefault.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.x, rect.y*i);
    }
    void ButtonCallback(UIButton button, ItemInput item)
    {
        if (item == _itemSelected)
        {
            _controller.ActivateBehaviour(101, false);
            _itemSelected._amount = 1;
            _itemSelected = null;
            _buttonCache.ActivateController("select", false);
            _buttonCache = null;
            return;
        } else
        {
            if (_buttonCache != null)
            {
                _buttonCache.ActivateController("select", false);
            }
            _controller.ActivateBehaviour(101, true);
            _itemSelected = item;
            _itemSelected._amount = 1;
            RefreshItemCraft();
            _buttonCache = button;
            _buttonCache.ActivateController("select", true);
        }
    }
    void RefreshItemCraft()
    {
        Item item = ItemHandler.Instance.GetUnlinkedItem(_itemSelected._index);
        _controller.SetText(item._options._description, "item description");
        _controller.SetText(_itemSelected._amount.ToString(), "item amount");
    }
    void CraftCallback()
    {
        if (_itemSelected != default(ItemInput))
        {
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Craft, _itemSelected);
            _buttonCache.ActivateController("select", false);
            _buttonCache = null;
            _itemSelected = default(ItemInput);
            _controller.ActivateBehaviour(101, false);
        }
    }
}

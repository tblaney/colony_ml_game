using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIBuildingStorage : UIBuilding
{
    [SerializeField] private UIButton _buttonItemDefault;
    [SerializeField] private UIButton _buttonInventorySendDefault;
    ItemInventory _inventory;
    Item _itemSelected = null;

    public override void SetupBuilding()
    {
        //base.InitializeBuilding();
        _inventory = ItemHandler.Instance.GetItemInventory(_node._inventoryIndex);
        _controller.SetText("Inventory: " + _inventory._index.ToString(), "inventory name");
        //ActivateInventories(false);
    }
    public override void ActivateBuilding(bool val)
    {
        if (val)
        {
            RefreshInventories();
        } else
        {
            ActivateInventories(false);
        }
    }
    public override void Refresh()
    {
        RefreshItems();
    }
    void RefreshItems()
    {
        List<Item> items = _inventory._items;
        Vector2 rect = _buttonItemDefault.GetComponent<RectTransform>().sizeDelta;
        int i = 0;
        foreach (Item item in items)
        {
            if (item._amount == 0)
                continue;
            
            UIButton button = Instantiate(_buttonItemDefault, _buttonItemDefault.transform.parent);
            button.gameObject.SetActive(true);
            UIController controller = button.GetComponent<UIController>();
            button.OnPointerClickFunc = () =>
            {
                ButtonCallback(item);
            };
            controller.FormList();
            controller.SetText(item._name, "name");
            controller.SetText(item._amount.ToString(), "amount");
            i++;
        }
        _buttonItemDefault.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.x, rect.y*i);
    }
    void RefreshInventories()
    {
        List<int> inventories = HabitationHandler.Instance.GetItemInventories(false);
        if (inventories.Contains(_inventory._index))
            inventories.Remove(_inventory._index);
        Vector2 rect = _buttonInventorySendDefault.GetComponent<RectTransform>().sizeDelta;
        int y = 0;
        foreach (int i in inventories)
        {
            UIButton button = Instantiate(_buttonInventorySendDefault, _buttonInventorySendDefault.transform.parent);
            button.gameObject.SetActive(true);
            UIController controller = button.GetComponent<UIController>();
            button.OnPointerClickFunc = () =>
            {
                InventoryCallback(i);
            };
            controller.FormList();
            controller.SetText("Send To: Inventory " + i.ToString(), "name");
            y++;
        }
        _buttonInventorySendDefault.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(rect.x, rect.y*y);
    }
    void ActivateInventories(bool active)
    {
        if (active)
        {
            _controller.ActivateBehaviour("inventory options", true);
        } else
        {
            _controller.ActivateBehaviour("inventory options", false);
        }
    }
    void ButtonCallback(Item item)
    {
        if (_itemSelected != null)
        {
            if (_itemSelected == item)
            {
                ActivateInventories(false);
                return;
            }
        }
        _itemSelected = item;
        ActivateInventories(true);
    }
    void InventoryCallback(int index)
    {
        if (_itemSelected != null)
        {   
            // send item selected to index of inventory
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Haul, new HaulQueueable(_itemSelected.GetItemInput(), _inventory._index, index){});
            _buttonClose.OnPointerClickFunc();
        }
    }
}

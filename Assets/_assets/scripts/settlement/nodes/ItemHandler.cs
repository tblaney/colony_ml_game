using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemHandler : MonoBehaviour, IHandler
{
    public static ItemHandler Instance;
    [Header("Inputs:")]
    public List<Item> _items;
    public List<ItemParameter> _parameters;
    public List<ItemInventory> _inventories;

    public void Initialize()
    {
        Instance = this;
        _inventories = new List<ItemInventory>();
    }
    public ItemInventory GenerateDefaultItemInventory()
    {
        ItemInventory inventory = new ItemInventory(GetOpenIndex(), 100);
        foreach (Item item in _items)
        {
            inventory._items.Add(item);
        }
        return inventory;
    }
    public ItemParameter GetParameter(string name)
    {
        foreach (ItemParameter parameter in _parameters)
        {
            if (parameter._name == name)
                return parameter;
        }
        return null;
    }
    public ItemInventory GetItemInventory(int index)
    {
        foreach (ItemInventory inventory in _inventories)
        {
            if (inventory._index == index)
                return inventory;
        }
        return null;
    }
    public void LoadInventory(ItemInventory inventory)
    {
        _inventories.Add(inventory);
    }
    public int NewInventory(int itemCapacity = 100)
    {
        int val = GetOpenIndex();
        if (val != -1)
        {
            ItemInventory inventory = GenerateDefaultItemInventory();
            inventory._itemCapacity = itemCapacity;
            _inventories.Add(inventory);
            return inventory._index;
        }
        return -1;
    }
    public void DestroyInventory(int index)
    {
        ItemInventory inventory = GetItemInventory(index);
        if (inventory != null)
        {
            if (_inventories.Contains(inventory))
                _inventories.Remove(inventory);
        }
    }
    int GetOpenIndex()
    {
        List<int> currentInts = new List<int>();
        foreach (ItemInventory inventory in _inventories)
        {
            currentInts.Add(inventory._index);
        }
        for (int i = 0; i < 30; i++)
        {
            int val = UnityEngine.Random.Range(0, 500);
            if (!currentInts.Contains(val))
                return val;
        }
        return -1;
    }
}

[Serializable]
public class Item
{
    public string _name;
    public int _index;

    public int _amount;
    public Node.Type _type;

    public void Add(int val)
    {
        _amount += val;
    }
    public void Remove(int val)
    {
        _amount -= val;
        if (_amount < 0)
            _amount = 0;
    }
}

[Serializable]
public class ItemInventory
{
    // for saving
    public List<Item> _items;
    public int _itemCapacity;
    public int _index;
    public InventoryObject _obj;

    public ItemInventory(int index, int capacity)
    {
        _index = index;
        _itemCapacity = capacity;
        _items = new List<Item>();
    }
    public void ClaimInventory(InventoryObject obj)
    {
        _obj = obj;
    }
    public void AddItem(string name, int amount)
    {
        if (GetItemAmount() + amount > _itemCapacity)
            return;
        
        Item itemTemp = GetItem(name);
        if (itemTemp != null)
        {
            itemTemp.Add(amount);
        } 
    }
    public void AddItem(ItemInput itemInput)
    {
        AddItem(itemInput._name, itemInput._amount);
    }
    public void RemoveItem(string name, int amount)
    {
        Item itemTemp = GetItem(name);
        if (itemTemp != null)
        {
            itemTemp._amount -= amount;
            if (itemTemp._amount <= 0)
                _items.Remove(itemTemp);
        }
    }
    public void RemoveItem(ItemInput itemInput)
    {
        RemoveItem(itemInput._name, itemInput._amount);
    }
    public bool Contains(int resourceIndex, int amount)
    {
        Item item = GetItem(resourceIndex);
        if (item != null)
        {
            if (item._amount >= amount)
                return true;
        } 
        return false;
    }
    Item GetItem(int index)
    {
        foreach (Item item in _items)
        {
            if (item._index == index)
                return item;
        }
        return null;
    }
    Item GetItem(string index)
    {
        foreach (Item item in _items)
        {
            if (item._name == index)
                return item;
        }
        return null;
    }
    int GetItemAmount()
    {
        int i = 0 ;
        foreach (Item item in _items)
        {
            i += item._amount;
        }
        return i;
    }
}

[Serializable]
public class ItemParameter
{
    public string _name;
    public Sprite _sprite;
    public string _description;
    public List<ItemInput> _recipe;
}

[Serializable]
public struct ItemInput
{
    public string _name;
    public int _amount;
}
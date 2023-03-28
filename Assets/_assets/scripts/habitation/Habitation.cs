using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Habitation
{
    // main class that stores all information regarding the habitation, should just be able to load this in from a save
    public List<HabBot> _bots;
    public List<int> _itemInventories;
    public static HabBotStateParameters _stateParameters;
    public void NewHabitation(Bounds restBounds)
    {
        _bots = new List<HabBot>();
        for (int i = 0; i < HabitationHandler._parameters._botAmountStart; i++)
        {
            HabBot bot = new HabBot();
            bot.InitializeRandom(i);
            _bots.Add(bot);
        }
        _itemInventories = new List<int>();
    }
    public void Initialize(HabBotStateParameters parameters)
    {
        _stateParameters = parameters;
        foreach (HabBot bot in _bots)
        {
            bot.Initialize();
        }
    }
    public HabBot GetClosestBot(Vector3 position)
    {
        float distanceMin = 1000f;
        HabBot botOut = null;
        foreach (HabBot bot in _bots)
        {
            float distance = Vector3.Distance(bot.GetPosition(), position);
            if (distance < distanceMin)
            {
                distanceMin = distance;
                botOut = bot;
            }
        }
        return botOut;
    }
    public List<Item> GetAllItems()
    {
        ItemInventory inventoryTemp = ItemHandler.Instance.GenerateDefaultItemInventory();
        List<ItemInventory> itemInventories = new List<ItemInventory>();
        foreach (int i in _itemInventories)
        {
            itemInventories.Add(ItemHandler.Instance.GetItemInventory(i));
        }
        // returns all items on bots and nodes
        foreach (ItemInventory inventory in itemInventories)
        {
            //items.AddRange(inventory._items);
            foreach (Item item in inventory._items)
            {
                if (item._amount == 0)
                    continue;
                inventoryTemp.AddItem(item._name, item._amount);
            }
        }
        return inventoryTemp._items;
    }
    public List<ItemInventory> GetItemInventoriesWithItem(ItemInput item)
    {
        List<ItemInventory> inventories = new List<ItemInventory>();
        foreach (int i in _itemInventories)
        {
            ItemInventory inventory = ItemHandler.Instance.GetItemInventory(i);
            if (inventory.Contains(item._index, item._amount))
            {
                inventories.Add(inventory);
            }
        }
        return inventories;
    }
    public void AddInventory(int index)
    {
        _itemInventories.Add(index);
    }
    public void RemoveInventory(int index)
    {
        if (_itemInventories.Contains(index))
            _itemInventories.Remove(index);
    }
    public Item GetItem(int index)
    {
        foreach (int j in _itemInventories)
        {
            Item item = ItemHandler.Instance.GetItem(index, j);
            if (item != null && item._amount > 0)
                return item;
        }
        return null;
    }
}
[Serializable]
public class HabBotStateParameters
{
    [Header("Inputs:")]
    public List<HabBotStateParameter> _parameters;
    public HabBotStateParameter GetParameter(HabBot.State state)
    {
        foreach (HabBotStateParameter param in _parameters)
        {
            if (param._state == state)
                return param;
        }
        return default(HabBotStateParameter);
    }
}

[Serializable]
public struct HabBotStateParameter
{
    [Header("Inputs:")]
    public string _name;
    public HabBot.State _state;
    public string _description;
    [Header("Options:")]
    [Tooltip("Closes the UI window to allow fast transition if selected")]
    public bool _closeOnSelect;
}
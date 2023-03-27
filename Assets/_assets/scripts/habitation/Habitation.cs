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
    public void Initialize()
    {
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
    public void AddInventory(int index)
    {
        _itemInventories.Add(index);
    }
    public void RemoveInventory(int index)
    {
        if (_itemInventories.Contains(index))
            _itemInventories.Remove(index);
    }
}

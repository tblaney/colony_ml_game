using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Habitation
{
    // main class that stores all information regarding the habitation, should just be able to load this in from a save
    public List<HabBot> _bots;
    public List<HabitationZone> _zones;
    public List<int> _itemInventories;

    public void NewHabitation(Bounds restBounds)
    {
        _bots = new List<HabBot>();
        _zones = new List<HabitationZone>();
        _zones.Add(new HabitationZone(){_type = HabitationZone.Type.Rest, _bounds = restBounds});
        for (int i = 0; i < HabitationProcessor._parameters._botAmountStart; i++)
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
    public Vector3 GetHabitationZonePosition(HabitationZone.Type zoneType)
    {
        HabitationZone zone = GetZone(zoneType);
        if (zone != null)
        {
            return zone.GetRandomPosition();
        }
        return default(Vector3);
    }
    public HabitationZone GetZone(HabitationZone.Type zoneType)
    {
        foreach (HabitationZone zone in _zones)
        {
            if (zone._type == zoneType)
                return zone;
        }
        return null;
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
    /*
    public void AddItem(ItemInput itemInput)
    {
        _itemInventory.AddItem(itemInput);
    }
    public void RemoveItem(string name, int amount)
    {
        _itemInventory.RemoveItem(name, amount);
    }
    */
}

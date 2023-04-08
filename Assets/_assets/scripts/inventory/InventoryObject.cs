using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObject : MonoBehaviour
{
    public int _inventoryIndex;
    public bool _habitation = false;
    public ItemInventory _inventory;
    NodeObject _nodeObject;

    public void Initialize(int inventoryIndex = 0, bool habitation = false)
    {
        _nodeObject = GetComponent<NodeObject>();
        if (inventoryIndex == 0)
        {
            inventoryIndex = ItemHandler.Instance.NewInventory();
        }
        _inventoryIndex = inventoryIndex;
        _habitation = habitation;
        _inventory = ItemHandler.Instance.GetItemInventory(_inventoryIndex);
        _inventory.ClaimInventory(this);
        if (_habitation)
        {
            HabitationHandler.Instance.AddInventory(_inventoryIndex);
        }
    }
    public void DestroyInventory()
    {
        if (_habitation)
        {
            HabitationHandler.Instance.RemoveInventory(_inventoryIndex);
        }
        ItemHandler.Instance.DestroyInventory(_inventoryIndex);
    }
    public void AddItem(ItemInput input)
    {
        _inventory.AddItem(input);
    }
    public void AddItem(int index, int amount)
    {
        _inventory.AddItem(index, amount);
    }
    public void RemoveItem(ItemInput input)
    {
        _inventory.RemoveItem(input);
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public Node GetNode()
    {
        return _nodeObject.GetNode();
    }
    public NodeObject GetNodeObject()
    {
        return _nodeObject;
    }
}

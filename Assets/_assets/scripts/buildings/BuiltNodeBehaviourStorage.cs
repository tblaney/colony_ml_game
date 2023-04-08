using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuiltNodeBehaviourStorage : BuiltNodeBehaviour
{
    public InventoryObject _inventoryObject;
    ItemInventory _inventory;
    public override void StartBehaviour()
    {
        if (_obj._node._inventoryIndex == 0)
        {
            _obj._node._inventoryIndex = ItemHandler.Instance.NewInventory();
        }
        _inventory = ItemHandler.Instance.GetItemInventory(_obj._node._inventoryIndex);
        _inventoryObject.Initialize(_obj._node._inventoryIndex, true);
    }
    public override void UpdateBehaviour()
    {

    }   
}

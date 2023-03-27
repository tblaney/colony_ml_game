using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuiltNodeBehaviourStorage : BuiltNodeBehaviour
{
    ItemInventory _inventory;
    public override void StartBehaviour()
    {
        if (_node._inventoryIndex == 0)
        {
            _node._inventoryIndex = ItemHandler.Instance.NewInventory();
        }
        _inventory = ItemHandler.Instance.GetItemInventory(_node._inventoryIndex);
    }
    public override void UpdateBehaviour()
    {

    }   
}

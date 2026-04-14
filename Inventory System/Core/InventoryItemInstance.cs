using System;
using UnityEngine;

public class InventoryItemInstance
{
    public string uid;
    public InventoryItem inventoryItem;
    public InventoryItemInstance(InventoryItem item)
    {
        inventoryItem = item;
        uid = Guid.NewGuid().ToString();
    }
}

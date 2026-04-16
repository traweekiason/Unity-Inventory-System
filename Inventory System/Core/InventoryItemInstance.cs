using System;
using UnityEngine;

public class InventoryItemInstance
{
    public string uid;
    public InventoryItem inventoryItem;
    public int stack = 0;
    public InventoryItemInstance(InventoryItem item, int initialStack = 1)
    {
        inventoryItem = item;
        stack = initialStack;
        uid = Guid.NewGuid().ToString();
    }
}

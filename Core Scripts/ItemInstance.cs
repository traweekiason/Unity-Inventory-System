using System;
using UnityEngine;

public class ItemInstance
{
    public string uid;
    public Item inventoryItem;
    public int stack = 0;
    public ItemInstance(Item item, int initialStack = 1)
    {
        inventoryItem = item;
        stack = initialStack;
        uid = Guid.NewGuid().ToString();
    }
}

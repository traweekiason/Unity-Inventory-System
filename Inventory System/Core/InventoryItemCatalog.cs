using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "InventoryItemCatalog")]
public class InventoryItemCatalog : ScriptableObject
{
    public static InventoryItemCatalog Instance {  get; private set; }
    public List<InventoryItem> items;


    public InventoryItem GetItem(string id)
    {
        foreach (InventoryItem item in items)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        return null;
    }
}

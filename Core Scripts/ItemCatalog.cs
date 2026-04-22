using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ItemCatalog")]
public class ItemCatalog : ScriptableObject
{
    public static ItemCatalog Instance {  get; private set; }
    public List<Item> items;


    public Item GetItem(string id)
    {
        foreach (Item item in items)
        {
            if (item.id == id)
            {
                return item;
            }
        }

        return null;
    }
}

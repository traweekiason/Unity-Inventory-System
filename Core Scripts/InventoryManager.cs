using UnityEngine;

public class InventoryManager 
{
    public bool TransferItem(Inventory from, Inventory to, ItemInstance item, Vector2Int toPos)
    {
        if (to == from)
        {
            from.MoveItem(toPos, item);
            return true;
        }
        else
        {
            if (to.PlaceItem(item, toPos) == true)
            {
                from.RemoveItem(item);
                return true;
            }

            // Debug.Log("Inventory 0 has " + from.ItemCount() + " items, Inventory 1 has " + to.ItemCount() + " items");
        }
        return false;
 
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class DropPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData data)
    {
        GameObject gameObject = data.pointerDrag;

        InventoryItemInstance item = gameObject.GetComponent<ItemUI>().GetItem();

        GameObject droppedItem = Instantiate(item.inventoryItem.gameObject, null);


        Inventory inventory = gameObject.GetComponent<ItemUI>().inventory;
        droppedItem.transform.position = inventory.dropPointObject.transform.position;
        inventory.RemoveItem(item);

        Destroy(gameObject);
        
    }
}

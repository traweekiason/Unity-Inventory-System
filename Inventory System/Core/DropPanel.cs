using UnityEngine;
using UnityEngine.EventSystems;

public class DropPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData data)
    {
        GameObject gameObject = data.pointerDrag;

        InventoryItemInstance item = gameObject.GetComponent<ItemUI>().GetItem();

        GameObject droppedItem = Instantiate(item.inventoryItem.worldObject, null);


        Inventory inventory = gameObject.GetComponent<ItemUI>().inventory;
        droppedItem.transform.position = inventory.dropPointObject.transform.position;
        droppedItem.transform.rotation = inventory.dropPointObject.transform.rotation;
        droppedItem.transform.Rotate(new Vector3(0, 90, 0 ));
        inventory.RemoveItem(item);

        Destroy(gameObject);
        
    }
}

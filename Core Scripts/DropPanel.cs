using UnityEngine;
using UnityEngine.EventSystems;

public class DropPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData data)
    {
        GameObject gameObject = data.pointerDrag;

        ItemInstance item = gameObject.GetComponent<ItemUI>().GetItem();
        Inventory inventory = gameObject.GetComponent<ItemUI>().inventory;

        inventory.DropItem(item, gameObject);
        
    }
}

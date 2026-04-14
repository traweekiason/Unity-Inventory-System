using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public bool occupied = false;

    InventoryManager inventoryManager = new InventoryManager();

    public void OnPointerEnter(PointerEventData data)
    {
        GetComponent<RawImage>().color = Color.beige;
    }
    public void OnPointerExit(PointerEventData data)
    {
        GetComponent<RawImage>().color = Color.white;
    }
    public void OnDrop(PointerEventData data)
    {
        
        GameObject draggedItem = data.pointerDrag;
        InventoryItemInstance item = draggedItem.GetComponent<ItemUI>().GetItem();
        Inventory from = draggedItem.GetComponent<ItemUI>().inventory;

        if (gridPosition == draggedItem.GetComponent<ItemUI>().gridPosition && inventory == from)
            return;
        

        if (inventoryManager.TransferItem(from, inventory, item, gridPosition))
            Destroy(draggedItem);
    }
}

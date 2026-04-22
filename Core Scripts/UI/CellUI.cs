using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public Color defaultColor;
    public Color hoverColor;
    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public Inventory inventory;

    RawImage rawImage;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();

        rawImage.color = defaultColor;
    }

    InventoryManager inventoryManager = new InventoryManager();

    public void OnPointerEnter(PointerEventData data)
    {
        rawImage.color = hoverColor;
    }
    public void OnPointerExit(PointerEventData data)
    {
       rawImage.color = defaultColor;
    }
    public void OnDrop(PointerEventData data)
    {
        
        GameObject draggedItem = data.pointerDrag;
        ItemInstance item = draggedItem.GetComponent<ItemUI>().GetItem();
        Inventory from = draggedItem.GetComponent<ItemUI>().inventory;

        if (gridPosition == draggedItem.GetComponent<ItemUI>().gridPosition && inventory == from)
            return;
        

        if (inventoryManager.TransferItem(from, inventory, item, gridPosition))
            Destroy(draggedItem);
    }
}

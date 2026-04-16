using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    
    [HideInInspector] public Vector2Int gridPosition; 
    [HideInInspector] public Vector2 position;

    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Transform parent;
    [HideInInspector] public Transform dragLayer;

    [HideInInspector] public bool isDragging = false;
    public Text text;

    public void Start()
    {
        text = GetComponentInChildren<Text>();
    }
    public void Update()
    {
        InventoryItemInstance instance = GetItem();

        if (instance != null)
        {   
            if (instance.inventoryItem.stackable)
            {
                text.text = "x" + (instance.stack);
            }
        }
    }

    public InventoryItemInstance GetItem()
    {
        InventoryItemInstance item = inventory.items[gridPosition.x, gridPosition.y];
        return item;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        GetComponent<RawImage>().raycastTarget = false;
        parent = transform.parent;
        transform.SetParent(dragLayer);

        isDragging = true;

    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }
    public void OnEndDrag(PointerEventData data)
    {
        isDragging = false;
        GetComponent<RawImage>().raycastTarget = true;
        transform.position = position;
        transform.SetParent(parent);

    }
    public virtual void OnDrop(PointerEventData data)
    {
        GameObject dragObject = data.pointerDrag;
        if (dragObject == null) return;

        ItemUI dragItemUI = dragObject.GetComponent<ItemUI>();
        if (dragItemUI == null) return;

        InventoryItemInstance dragItemInstance = dragItemUI.GetItem();
        InventoryItemInstance thisItem = GetItem();

        int moved = inventory.AddToStack(thisItem, dragItemInstance);

        if (moved > 0 && dragItemInstance.stack <= 0)
        {
            dragItemUI.inventory.RemoveItem(dragItemInstance);
            Destroy(dragObject);
        }
    }

}

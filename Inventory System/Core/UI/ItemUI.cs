using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler
{
    
    [HideInInspector] public Vector2Int gridPosition; 
    [HideInInspector] public Vector2 position;

    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Transform parent;
    [HideInInspector] public Transform dragLayer;

    [HideInInspector] public bool isDragging = false;


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
}

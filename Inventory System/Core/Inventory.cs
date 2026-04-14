using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [Header("Main")]
    public int width;
    public int height;
    public float maxWeight;
    public float maxVolume;

    [HideInInspector] public InventoryItemInstance[,] items;
    [HideInInspector] float currentWeight;
    [HideInInspector] float currentVolume;

    [Header("Drag-Ins")]
    public InventoryUI inventoryUI;
    public InventoryItemCatalog itemCatalog;
    public GameObject canvas;
    public GameObject dropPointObject;

    public event Action OnInventoryChanged;

    bool isVisible = false;


    // ─── Unity Lifecycle ──────────────────────────────────────────────────────

    void Start()
    {
        items = new InventoryItemInstance[width, height];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canvas.SetActive(false);

        OnInventoryChanged?.Invoke();
    }


    // ─── Public API ───────────────────────────────────────────────────────────

    public void InvokeOnInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void ToggleInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        isVisible = !isVisible;
        Cursor.visible = isVisible;

        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            canvas.SetActive(true);
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            canvas.SetActive(false);
        }

        inventoryUI.Refresh();
    }

    public bool CanPlaceItem(InventoryItemInstance item, Vector2Int position)
    {
        bool slotIsEmpty = items[position.x, position.y] == null;
        bool withinWeightLimit = currentWeight + item.inventoryItem.weight <= maxWeight;
        bool withinVolumeLimit = currentVolume + item.inventoryItem.volume <= maxVolume;

        return slotIsEmpty && withinWeightLimit && withinVolumeLimit;
    }

    public bool TryPlaceItem(InventoryItemInstance item, Vector2Int position)
    {
        if (!CanPlaceItem(item, position)) return false;

        items[position.x, position.y] = item;
        RecalculateWeightAndVolume();
        OnInventoryChanged?.Invoke();

        return true;
    }

    public void RemoveItem(InventoryItemInstance item)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (items[x, y] != null && items[x, y].uid == item.uid)
                {
                    items[x, y] = null;
                    RecalculateWeightAndVolume();
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }
        }
    }

    public bool MoveItem(Vector2Int destination, InventoryItemInstance item)
    {
        if (items[destination.x, destination.y] != null) return false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (items[x, y] != null && items[x, y].uid == item.uid)
                {
                    items[destination.x, destination.y] = items[x, y];
                    items[x, y] = null;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        return false;
    }

    public Vector2Int FindAvailablePosition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (items[x, y] == null) return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1);
    }

    public InventoryItemInstance CreateItem(string itemId)
    {
        InventoryItem definition = itemCatalog.GetItem(itemId);
        if (definition == null) return null;

        return new InventoryItemInstance(definition);
    }

    public int ItemCount()
    {
        int count = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (items[x, y] != null) count++;
            }
        }

        return count;
    }


    // ─── Private Helpers ──────────────────────────────────────────────────────

    void RecalculateWeightAndVolume()
    {
        currentWeight = 0;
        currentVolume = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (items[x, y] == null) continue;

                currentWeight += items[x, y].inventoryItem.weight;
                currentVolume += items[x, y].inventoryItem.volume;
            }
        }
    }
}
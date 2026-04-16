using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

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
            OnInventoryChanged?.Invoke();
        }

        inventoryUI.Refresh();
    }

    // ADDING AN ITEM FOLLOWS THIS PATH 
    // Note: TryPlaceItem() function can return void. 
    //
    //  1. Find a new grid position or the position of a stackable item with the same id as itemInstance
    //      - Iterate through the inventory. Nested loop for x and y
    //      - If an item is found, check if it is stackable
    //      -- If the item is stackable, return the position
    //      -- If the item is not stackable, repeat or return the next existing available grid position
    //      -- If no items are found, return -1. No space in inventory
    //  2. Check if the inventory can hold more items by volume or weight (grid slot was found)
    //  3. Try to place the item in a slot or add to an existing stack 
    //      - A position was found for the new item. (null case) 
    //      - Check if item is present at the position
    //      -- Item is present at the position, check if the item is stackable. 
    //      --- If the item is stackable, add to the stack (int)
    //      -- Item is not present. Set the itemInstance to the cell at the position
    //  4. Return true or false to check if it worked. 

    public Vector2Int FindAvailablePosition(InventoryItemInstance itemInstance)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (items[x,y] == null)
                {
                    return new Vector2Int(x,y);
                }
                else
                {
                    if (items[x,y].inventoryItem.id == itemInstance.inventoryItem.id && items[x,y].inventoryItem.stackable == true)
                    {
                        if (items[x,y].stack < items[x,y].inventoryItem.maxStack)
                            return new Vector2Int(x,y);

                    }
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    public bool CanPlaceItem(InventoryItemInstance item, Vector2Int position)
    {
        bool withinWeightLimit = currentWeight + item.inventoryItem.weight <= maxWeight;
        bool withinVolumeLimit = currentVolume + item.inventoryItem.volume <= maxVolume;

        bool positionInIndex = true;

        if (position == new Vector2Int(-1, -1))
        {
            positionInIndex = false;
        }

        return withinWeightLimit && withinVolumeLimit && positionInIndex;
    }

    public bool TryPlaceItem(InventoryItemInstance item, Vector2Int position)
    {
        if (!CanPlaceItem(item, position)) return false;

        if (items[position.x, position.y] != null)
        {
            items[position.x, position.y].stack += item.stack; // ← instance stack
            // print(items[position.x, position.y].stack);
            RecalculateWeightAndVolume();
            OnInventoryChanged?.Invoke();
        }
        else
        {
            items[position.x, position.y] = item;
            OnInventoryChanged?.Invoke();
        }
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

    public int AddToStack(InventoryItemInstance target, InventoryItemInstance source)
    {
        if (target == null || source == null) return 0;

        if (target.inventoryItem.id != source.inventoryItem.id) return 0;

        if (!target.inventoryItem.stackable) return 0;

        int maxStack = target.inventoryItem.maxStack;

        int spaceLeft = maxStack - target.stack;
        if (spaceLeft <= 0) return 0;

        // Determine transfer amount
        int amountToMove = Mathf.Min(spaceLeft, source.stack);

        // Apply transfer
        target.stack += amountToMove;
        source.stack -= amountToMove;

        // Recalculate stats
        RecalculateWeightAndVolume();
        OnInventoryChanged?.Invoke();

        return amountToMove;
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

                if (items[x,y].inventoryItem.stackable)
                {
                    currentWeight += items[x, y].inventoryItem.weight * items[x,y].stack;
                    currentVolume += items[x, y].inventoryItem.volume * items[x,y].stack;
                }
                else
                {
                    currentWeight += items[x, y].inventoryItem.weight;
                    currentVolume += items[x, y].inventoryItem.volume;
                }
 
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Drag-Ins")]
    public Inventory inventory;
    public GameObject cellPrefab;
    public GameObject itemPrefab;

    public Transform dragLayer;
    [HideInInspector]GameObject cellContainer;
    [HideInInspector]public GameObject itemContainer;

    GameObject[,] cells;
    List<GameObject> spawnedItems;

    bool hasStarted = false;


    // ─── Unity Lifecycle ──────────────────────────────────────────────────────


    private void Awake()
    {
        spawnedItems = new List<GameObject>();
    }

    private void Start()
    {
        cellContainer = transform.GetChild(0).gameObject;
        itemContainer = transform.GetChild(1).gameObject;

        GetComponent<RectTransform>().sizeDelta = new Vector2(64 * inventory.width, 64 * inventory.height);

        GridLayoutGroup grid = cellContainer.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = inventory.width;

        BuildGrid(inventory.width, inventory.height);

        hasStarted = true;
    }

    private void OnEnable()
    {
        if (!hasStarted) return;

        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
        
        Refresh();
    }


    // ─── Private Helpers ──────────────────────────────────────────────────────

    private void BuildGrid(int width, int height)
    {
        // Destroy all existing cell children safely in both edit and play mode
        for (int i = cellContainer.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = cellContainer.transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        cells = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newCell = Instantiate(cellPrefab, cellContainer.transform);

                CellUI cellScript = newCell.GetComponent<CellUI>();
                cellScript.inventory = inventory;
                cellScript.gridPosition = new Vector2Int(x, y);

                cells[x, y] = newCell;
            }
        }
    }

    public virtual void Refresh()
    {
        if (inventory == null || cells == null) return;

        // In InventoryUI.Refresh()
        foreach (Transform child in itemContainer.transform)
        {
            ItemUI ui = child.GetComponent<ItemUI>();

            // Don't destroy the item currently being dragged
            if (ui != null && ui.isDragging)
                continue;

            Destroy(child.gameObject);
        }

        spawnedItems.Clear();

        for (int x = 0; x < inventory.width; x++)
        {
            for (int y = 0; y < inventory.height; y++)
            {
                if (inventory.items == null || inventory.items[x, y] == null) continue;
                if (cells[x, y] == null) continue;

                GameObject newItem = Instantiate(itemPrefab, itemContainer.transform);

                if (newItem == null) continue;

                ItemInstance itemInstance = inventory.items[x, y];
                RectTransform cellRect = cells[x, y].GetComponent<RectTransform>();

                ItemUI itemUI = newItem.GetComponent<ItemUI>();

                newItem.GetComponent<RawImage>().texture = itemInstance.inventoryItem.icon;

                itemUI.gridPosition = new Vector2Int(x, y);
                itemUI.inventory = inventory;
                itemUI.dragLayer = dragLayer;
                itemUI.position = cells[x, y].transform.position;

                itemUI.transform.position = cells[x,y].transform.position;

                newItem.GetComponent<RectTransform>().position = cellRect.position;

                spawnedItems.Add(newItem);

            }
        }
    }
}
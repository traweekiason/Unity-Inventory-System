using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interract : MonoBehaviour
{
    public List<Inventory> inventories = new List<Inventory>();

    GameObject current;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            if (current != hit.collider.gameObject)
            {
                current = hit.collider.gameObject;
                // Debug.Log("Looking at: " + current.name);
            }
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (current)
            {

                ItemWorldObject worldObject = current.GetComponent<ItemWorldObject>();
                if (worldObject != null)
                {
                    foreach (Inventory inventory in inventories)
                    {
                        Vector2Int position = inventory.FindAvailablePosition();
                        if (position != new Vector2(-1, -1))
                        {
                            if (worldObject.itemInstance != null)
                            {
                                if (inventory.TryPlaceItem(worldObject.itemInstance, position))
                                {
                                    Destroy(current);
                                    return;
                                }
                            }
                            else
                            {
                                InventoryItemInstance item = inventory.CreateItem(worldObject.id);
                                if (inventory.TryPlaceItem(item, position))
                                {
                                    Destroy(current);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

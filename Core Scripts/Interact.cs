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
        if (ctx.performed && current && current.GetComponent<WorldObject>())
        {

            foreach (Inventory inventory in inventories)
            {
                WorldObject worldObject = current.GetComponent<WorldObject>();
                ItemInstance itemInstance = worldObject.itemInstance;

                if (itemInstance == null)
                {
                    itemInstance = inventory.CreateItem(worldObject.id);
                    current.GetComponent<WorldObject>().itemInstance = itemInstance;
                }

                if (inventory.FindAvailablePosition(itemInstance) != new Vector2(-1, -1) && inventory.PlaceItem(itemInstance, inventory.FindAvailablePosition(itemInstance)))
                {
                    Destroy(current.gameObject);
                    return;
                }

            }
        }
    }
}

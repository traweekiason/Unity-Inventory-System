using UnityEngine;
[CreateAssetMenu(menuName = "InventoryItem/Default")]
public class InventoryItem : ScriptableObject
{
    public string id;
    public float weight;
    public float volume;
    public bool stackable;
    public int maxStack;
    public string type;
    public Texture icon;
    public GameObject worldObject;
    public GameObject handObject;

    public Vector3 defaultPosition;
    public ItemAction[] actions;
}

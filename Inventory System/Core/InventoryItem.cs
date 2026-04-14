using UnityEngine;
[CreateAssetMenu(menuName = "InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string id;
    public float weight;
    public float volume;

    public Texture icon;
    public GameObject gameObject;
}

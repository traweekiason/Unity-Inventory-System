using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory Item/Default")]

public class Item : ScriptableObject
{
    public string id;
    public float weight;
    public float volume;
    public bool stackable;
    public int maxStack;
    public Texture icon;
    public GameObject worldObject;

    public enum Type
    {
        Weapon,
        Consumable
    };

    public Type type;

    

}

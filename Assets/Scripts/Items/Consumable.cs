using UnityEngine;
using System;
using static Enums;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class Consumable : Item
{
    // Consumable-specific functionality can go here
    [Header("Consumable Properties")]
    public string useVerb;
    public int uses;

}


[Serializable]
public class ConsumableData
{
    public string itemName;
    public string itemDescription;
    public ItemType itemType; // e.g., "Weapon", "Armor"
    public string image;
    public string statsText;

    public int STR;
    public int SPD;
    public int AGI;
    public int HP;
    public string useVerb; 
    public int uses; 
}

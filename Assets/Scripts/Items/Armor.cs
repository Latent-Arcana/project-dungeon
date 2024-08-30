using System;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : Item
{
    [Header("Armor Stats")]
    // Armor-specific functionality can go here
    public int AP;
}


[Serializable]
public class ArmorData
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
    public int AP; 
}

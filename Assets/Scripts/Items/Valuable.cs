using UnityEngine;
using System;
using static Enums;

[CreateAssetMenu(menuName = "Items/Valuable")]
public class Valuable : Item {
    
    [Header("Valuable Item Stats")]
    public int value;
}


[Serializable]
public class ValuableData
{
    public string itemID;
    public string itemName;
    public string itemDescription;
    public ItemType itemType; // e.g., "Weapon", "Armor"
    public string image;
    public int STR;
    public int SPD;
    public int AGI;
    public int HP;
    public int value;
}

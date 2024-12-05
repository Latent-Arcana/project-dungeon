using System;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{
    [Header("Weapon Properties")]
    public int AP;
    public WeaponType weaponType;
}

[Serializable]
public class WeaponData
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
    public int AP;
    public string weaponType;
}

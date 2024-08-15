using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : Item
{
    [Header("Armor Stats")]
    // Armor-specific functionality can go here
    public int AP;
}

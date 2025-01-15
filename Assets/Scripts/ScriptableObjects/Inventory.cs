using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public List<Item> items;
    public int equippedArmor;
    public int equippedWeapon;


    [SerializeField]
    [Header("DEBUG")]
    [Tooltip("Reset the inventory on scene load?")]
    bool DEBUG_RESET = false;


    public void OnEnable()
    {
        if (DEBUG_RESET)
        {
            Reset();
        }

    }

    public void Reset()
    {
        items.Clear();

        equippedArmor = -1;
        equippedWeapon = -1;
    }
}
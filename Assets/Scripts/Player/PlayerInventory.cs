using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Use inheritance so that our PlayerInventory and ContainerBehavior don't have so much code dupe between them
public class PlayerInventory : MonoBehaviour
{

    [SerializeField]
    public int maxItemCount; // this is where we're going to define how many items at a time can spawn in our object

    [SerializeField]
    public PlayerStatsManager Player_Stats;

    public int equippedArmor = -1;
    public int equippedWeapon = -1;

    public List<Item> items;

    // TODO: INVENTORY INTERACTION
    public List<Item> Open()
    {
        return items;
    }

    public void RemoveItem(int index)
    {
        if (items.Count > index)
        {
            items.RemoveAt(index);
        }
    }

    void AddItem(Item item)
    {
        items.Add(item);
    }

    public int EquipArmor(int index)
    {
        int indexToSwap = equippedArmor;
        equippedArmor = index;

        return indexToSwap;
    }

    public void UnequipArmor(int index)
    {

        if (equippedArmor == index)
        {
            equippedArmor = -1;

        }

    }

    public int EquipWeapon(int index)
    {
        int indexToSwap = equippedWeapon;
        equippedWeapon = index;

        return indexToSwap;
    }

    public void UnequipWeapon(int index)
    {
        if (equippedWeapon == index)
        {
            equippedWeapon = -1;

        }
    }

    public void Start(){
        equippedArmor = -1;
        equippedWeapon = -1;
    }

    public void Update(){
        Debug.Log("Armor: " + equippedArmor);
        Debug.Log("Weapon: " + equippedWeapon);
    }

    // DEBUG
    public void InventoryDebugPrint()
    {

        Debug.Log("_________________________________________________");

        Debug.Log($"The player is currently holding: ");

        foreach (Item item in items)
        {

            Debug.Log(item.itemName);
        }

        Debug.Log("_________________________________________________");
    }


}

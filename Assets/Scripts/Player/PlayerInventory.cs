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

            if(equippedArmor == index){

                equippedArmor = -1;
            }

            else if(equippedWeapon == index){
                equippedWeapon = -1;

            }
        }
    }

    void AddItem(Item item)
    {
        items.Add(item);
    }

    public void EquipItem(int index)
    {

        // out of bounds check
        if (index < 0 || index >= items.Count)
        {
            return;
        }

        // if we're handling armor
        if (items[index].type == Enums.ItemType.Armor)
        {

            equippedArmor = index;
        }


        // if we're handling weapons
        else if (items[index].type == Enums.ItemType.Weapon)
        {
            equippedWeapon = index;
        }

    }

    public void HandleUnequip(int index)
    {
        if (index == equippedArmor)
        {
            equippedArmor = -1;
        }

        else if (index == equippedWeapon)
        {
            equippedWeapon = -1;
        }
    }

    public int GetEquippedArmor()
    {
        return equippedArmor;
    }

    public int GetEquippedWeapon()
    {
        return equippedWeapon;
    }




    public void Start()
    {
        equippedArmor = -1;
        equippedWeapon = -1;
    }

    public void Update()
    {
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

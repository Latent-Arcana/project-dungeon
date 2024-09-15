using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

            if (equippedArmor == index)
            {

                equippedArmor = -1;
            }

            else if (equippedWeapon == index)
            {
                equippedWeapon = -1;

            }

            // now let's move our indices too
            if (index < equippedArmor)
            {
                equippedArmor -= 1;
            }

            if (index < equippedWeapon)
            {
                equippedWeapon -= 1;
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
            EquipStatsChange(items[index]);
        }


        // if we're handling weapons
        else if (items[index].type == Enums.ItemType.Weapon)
        {
            equippedWeapon = index;
            EquipStatsChange(items[index]);
        }

    }

    public void HandleUnequip(int index)
    {
        if (index == equippedArmor)
        {
            UnequipStatsChange(items[index]);
            equippedArmor = -1;
        }

        else if (index == equippedWeapon)
        {
            UnequipStatsChange(items[index]);
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

    public void EquipStatsChange(Item item)
    {

        Player_Stats.SetAGI(item.AGI + Player_Stats.AGI);
        Player_Stats.SetSPD(item.SPD + Player_Stats.SPD);
        Player_Stats.SetSTR(item.STR + Player_Stats.STR);
        Player_Stats.SetMaxHP(item.HP + Player_Stats.MAX_HP);

        if (item.type == Enums.ItemType.Armor)
        {
            Armor armor = item as Armor;
            Player_Stats.SetAP(armor.AP + Player_Stats.AP);
        }
    }

    public void UnequipStatsChange(Item item)
    {
        Player_Stats.SetAGI(Player_Stats.AGI - item.AGI);
        Player_Stats.SetSPD(Player_Stats.SPD - item.SPD);
        Player_Stats.SetSTR(Player_Stats.STR - item.STR);
        Player_Stats.SetMaxHP(Player_Stats.MAX_HP - item.HP);

        if (item.type == Enums.ItemType.Armor)
        {
            Armor armor = item as Armor;
            Player_Stats.SetAP(Player_Stats.AP - armor.AP);
        }
    }

    public void Start()
    {
        equippedArmor = -1;
        equippedWeapon = -1;
    }

    public void Update()
    {
        // Debug.Log("Armor: " + equippedArmor);
        // Debug.Log("Weapon: " + equippedWeapon);
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

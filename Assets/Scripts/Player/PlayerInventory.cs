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

    public Inventory inventory;

    // TODO: INVENTORY INTERACTION
    public List<Item> Open()
    {
        return inventory.items;
    }

    public void RemoveItem(int index)
    {
        if (inventory.items.Count > index)
        {
            inventory.items.RemoveAt(index);

            if (inventory.equippedArmor == index)
            {

                inventory.equippedArmor = -1;
            }

            else if (inventory.equippedWeapon == index)
            {
                inventory.equippedWeapon = -1;

            }

            // now let's move our indices too
            if (index < inventory.equippedArmor)
            {
                inventory.equippedArmor -= 1;
            }

            if (index < inventory.equippedWeapon)
            {
                inventory.equippedWeapon -= 1;
            }
        }
    }

    void AddItem(Item item)
    {
        inventory.items.Add(item);
    }

    public void Consume(int index)
    {
        Debug.Log("consume the " + inventory.items[index].itemName);
        EquipStatsChange(inventory.items[index]);
    }

    public void EquipItem(int index)
    {

        // out of bounds check
        if (index < 0 || index >= inventory.items.Count)
        {
            return;
        }

        // if we're handling armor
        if (inventory.items[index].type == Enums.ItemType.Armor)
        {

            inventory.equippedArmor = index;
            EquipStatsChange(inventory.items[index]);
        }


        // if we're handling weapons
        else if (inventory.items[index].type == Enums.ItemType.Weapon)
        {
            inventory.equippedWeapon = index;
            EquipStatsChange(inventory.items[index]);
        }

    }

    public void HandleUnequip(int index)
    {
        if (index == inventory.equippedArmor)
        {
            UnequipStatsChange(inventory.items[index]);
            inventory.equippedArmor = -1;
        }

        else if (index == inventory.equippedWeapon)
        {
            UnequipStatsChange(inventory.items[index]);
            inventory.equippedWeapon = -1;
        }
    }

    public int GetEquippedArmor()
    {
        return inventory.equippedArmor;
    }

    public int GetEquippedWeapon()
    {
        return inventory.equippedWeapon;
    }
    public void EquipStatsChange(Item item)
    {
        bool useMaxHealth = true;
        if (item.type == Enums.ItemType.Consumable)
        {
            Consumable consumable = item as Consumable;

            if (!consumable.permanent)
            {
                useMaxHealth = false;
            }
        }

        Player_Stats.SetAGI(item.AGI + Player_Stats.AGI);
        Player_Stats.SetSPD(item.SPD + Player_Stats.SPD);
        Player_Stats.SetSTR(item.STR + Player_Stats.STR);

        if (useMaxHealth)
        {
            Player_Stats.SetMaxHP(item.HP + Player_Stats.MAX_HP);
        }
        else
        {
            Player_Stats.SetHP(item.HP + Player_Stats.HP);

        }

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

        foreach (Item item in inventory.items)
        {

            Debug.Log(item.itemName);
        }

        Debug.Log("_________________________________________________");
    }


}

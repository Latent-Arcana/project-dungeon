using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContainerGeneration;
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
        /*
            [Armor1, Weapon1]
            equippedArmor = 0;
            equippedWeapon = 1;
            index = 0;
            DROP ITEM

            [Weapon1] (actual weapon is 0)

            equippedArmor -1;
            equippedWeapon = 0;

        */
        if (inventory.items.Count > index)
        {
            inventory.items.RemoveAt(index);
            inventory.currentDurability.RemoveAt(index);

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
        DungeonNarrator.Dungeon_Narrator.AddPotionConsumeText(inventory.items[index] as Consumable);
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

            DungeonNarrator.Dungeon_Narrator.AddArmorEquipText(inventory.items[index] as Armor);

        }


        // if we're handling weapons
        else if (inventory.items[index].type == Enums.ItemType.Weapon)
        {
            inventory.equippedWeapon = index;
            EquipStatsChange(inventory.items[index]);

            DungeonNarrator.Dungeon_Narrator.AddWeaponEquipText(inventory.items[index] as Weapon);
        }

    }

    public void HandleDropItemNarration(int index)
    {

        Item droppedItem = inventory.items[index];

        DungeonNarrator.Dungeon_Narrator.AddItemDropText(droppedItem);

    }

    public void HandleBrokenItemNarration(Item item){ // TODO: IMPROVE

        DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"{item.itemName} has broken.");
    }

    public void HandleUnequip(int index)
    {
        if (index == inventory.equippedArmor)
        {
            DungeonNarrator.Dungeon_Narrator.AddArmorUnequipText(inventory.items[index] as Armor);

            UnequipStatsChange(inventory.items[index]);
            inventory.equippedArmor = -1;
        }

        else if (index == inventory.equippedWeapon)
        {
            DungeonNarrator.Dungeon_Narrator.AddWeaponUnequipText(inventory.items[index] as Weapon);

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
            Player_Stats.SetHP(item.HP + Player_Stats.HP, sourceObjectName: gameObject.name);

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

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Item item = Container_Generator.GetRandomItem();

            if (inventory.items.Count < maxItemCount)
            {
                inventory.items.Add(item);

            }

        }
    }

    public void ReduceDurability(int index)
    {
        if (index >= 0 && index <= inventory.currentDurability.Count)
        {
            inventory.currentDurability[index]--;

            if(inventory.currentDurability[index] <= 0){

                HandleBrokenItemNarration(inventory.items[index]);
                RemoveItem(index);
            }
        }
    }

    public void SetDurability(int index, int value)
    {
        if (index >= 0 && index <= inventory.currentDurability.Count)
        {
            inventory.currentDurability[index] = value;
        }
    }
    public int GetDurability(int index)
    {
        Item inventoryItem = inventory.items[index];

        Weapon weapon = inventoryItem as Weapon;
        Armor armor = inventoryItem as Armor;

        if (weapon != null)
        {
            int itemDurability = weapon.DUR;
            int currentDurability = inventory.currentDurability[index];

            float durabilityPercentage = (float)currentDurability / itemDurability;

            return DurabilityCalculator(durabilityPercentage);
        }

        else if (armor != null)
        {
            int itemDurability = armor.DUR;
            int currentDurability = inventory.currentDurability[index];

            float durabilityPercentage = (float)currentDurability / itemDurability;

            return DurabilityCalculator(durabilityPercentage);
        }

        else
        {
            return 0;
        }
    }

    private int DurabilityCalculator(float durabilityPercentage)
    {
        int result;

        if (durabilityPercentage <= .20f)
        {
            result = 1;
        }
        else if (durabilityPercentage <= .40f)
        {
            result = 2;
        }
        else if (durabilityPercentage <= .60f)
        {
            result = 3;
        }
        else if (durabilityPercentage <= .80f)
        {
            result = 4;
        }
        else
        {
            result = 5;
        }

        //Debug.Log(result);

        return result;
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

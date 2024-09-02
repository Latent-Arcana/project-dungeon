using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// This class is used to load data from the items .json files
/// It gets called and used by the GameSetup controller and its data gets sent to ContainerController so that 
/// we can generate the items that go into the game.
/// </summary>
public class ItemLoader : MonoBehaviour
{
    [Header("Item Data JSON Files")]

    public TextAsset consumablesFile;
    public TextAsset weaponsFile;
    public TextAsset armorFile;

    private List<Item> itemsDatabase = new List<Item>();

    public static ItemLoader Item_Loader { get; set; }

    public List<Item> GetItemsDatabase()
    {
        return itemsDatabase;
    }

    void Awake()
    {
        // SINGLETON CHECK
        if (Item_Loader == null)
        {
            Item_Loader = this;
        }
        else if (Item_Loader != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(Item_Loader);
    }

    public void LoadItemsFromJson()
    {

        // Check for null files and log errors early
        if (consumablesFile == null)
        {
            Debug.LogError("JSON file for consumables was not assigned.");
            return;
        }

        if (weaponsFile == null)
        {
            Debug.LogError("JSON file for weapons was not assigned.");
            return;
        }

        if (armorFile == null)
        {
            Debug.LogError("JSON file for armor was not assigned.");
            return;
        }

        // All files are valid, proceed with processing
        string consumablesJsonString = consumablesFile.text;
        string weaponsJsonString = weaponsFile.text;
        string armorJsonString = armorFile.text;

        ConsumableData[] consumablesData = JsonUtility.FromJson<ConsumablesDataArray>(consumablesJsonString).consumables;
        WeaponData[] weaponsData = JsonUtility.FromJson<WeaponsDataArray>(weaponsJsonString).weapons;
        ArmorData[] armorData = JsonUtility.FromJson<ArmorDataArray>(armorJsonString).armor;

        CreateArmor(armorData);
        CreateConsumables(consumablesData);
        CreateWeapons(weaponsData);
    }

    private void CreateArmor(ArmorData[] dataArray)
    {
        foreach (ArmorData data in dataArray)
        {

            Armor armor = ScriptableObject.CreateInstance<Armor>();
            armor.itemName = data.itemName;
            armor.itemDescription = data.itemDescription;
            armor.AGI = data.AGI;
            armor.SPD = data.SPD;
            armor.STR = data.STR;
            armor.AP = data.AP;
            armor.HP = data.HP;
            armor.type = Enums.ItemType.Weapon;
            armor.image = data.image;
            armor.statsText = FormatStatsText(data.AGI, data.STR, data.SPD, data.AP, data.HP);

            if (armor != null)
            {
                itemsDatabase.Add(armor);

                //Debug.Log("Loaded armor " + armor.itemName + " and added to the dictionary.");
            }

        }
    }

    private void CreateWeapons(WeaponData[] dataArray)
    {

        foreach (WeaponData data in dataArray)
        {
            Weapon weapon = ScriptableObject.CreateInstance<Weapon>();
            weapon.itemName = data.itemName;
            weapon.itemDescription = data.itemDescription;
            weapon.AGI = data.AGI;
            weapon.SPD = data.SPD;
            weapon.STR = data.STR;
            weapon.useVerb = data.useVerb;
            weapon.HP = data.HP;
            weapon.AP = data.AP;
            weapon.type = Enums.ItemType.Weapon;
            weapon.image = data.image;
            weapon.statsText = FormatStatsText(data.AGI, data.STR, data.SPD, 0, data.HP);

            if (weapon != null)
            {
                itemsDatabase.Add(weapon);

                //Debug.Log("Loaded weapon " + weapon.itemName + " and added to the dictionary.");
            }

        }


    }

    private void CreateConsumables(ConsumableData[] dataArray)
    {
        foreach (ConsumableData data in dataArray)
        {
            Consumable consumable = ScriptableObject.CreateInstance<Consumable>();
            consumable.itemName = data.itemName;
            consumable.itemDescription = data.itemDescription;
            consumable.AGI = data.AGI;
            consumable.SPD = data.SPD;
            consumable.STR = data.STR;
            consumable.permanent = data.permanent == 1 ? true : false; // seriously I just want to convert 0 -> false
            consumable.uses = data.uses;
            consumable.useVerb = data.useVerb;
            consumable.HP = data.HP;
            consumable.type = Enums.ItemType.Consumable;
            consumable.image = data.image;
            consumable.statsText = FormatStatsText(data.AGI, data.STR, data.SPD, 0, data.HP);

            if (consumable != null)
            {
                itemsDatabase.Add(consumable);

                //Debug.Log("Loaded consumable item " + consumable.itemName + " and added to the dictionary.");
            }
        }

    }

    private string FormatStatsText(int AGI, int STR, int SPD, int AP, int HP)
    {
        string statsText = "";

        if (AGI > 0)
        {
            statsText += "+" + AGI.ToString() + " AGI  ";
        }

        else if (AGI < 0)
        {
            statsText += AGI.ToString() + " AGI  ";
        }

        if (STR > 0)
        {
            statsText += "+" + STR.ToString() + " STR  ";
        }
        else if (STR < 0)
        {
            statsText += STR.ToString() + " STR  ";
        }


        if (SPD > 0)
        {
            statsText += "+" + SPD.ToString() + " SPD  ";
        }
        else if (SPD < 0)
        {
            statsText += SPD.ToString() + " SPD  ";
        }


        if (AP > 0)
        {
            statsText += "+" + AP.ToString() + " AP  ";
        }
        else if (AP < 0)
        {
            statsText += AP.ToString() + " AP  ";
        }

        if (HP > 0)
        {
            statsText += "+" + HP.ToString() + " HP  ";
        }
        else if (HP < 0)
        {
            statsText += HP.ToString() + " HP  ";
        }

        return statsText;

    }

}

[Serializable]
public class ConsumablesDataArray
{
    public ConsumableData[] consumables;
}

[Serializable]
public class WeaponsDataArray
{
    public WeaponData[] weapons;
}

[Serializable]
public class ArmorDataArray
{
    public ArmorData[] armor;
}
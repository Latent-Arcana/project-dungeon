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

    void Awake(){
        // SINGLETON CHECK
        if(Item_Loader == null){
            Item_Loader = this;
        }
        else if(Item_Loader != this){
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

            if (armor != null)
            {
                itemsDatabase.Add(armor);

                Debug.Log("Loaded armor " + armor.itemName + " and added to the dictionary.");
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
            weapon.type = Enums.ItemType.Weapon;

            if (weapon != null)
            {
                itemsDatabase.Add(weapon);

                Debug.Log("Loaded weapon " + weapon.itemName + " and added to the dictionary.");
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
            consumable.uses = data.uses;
            consumable.useVerb = data.useVerb;
            consumable.HP = data.HP;
            consumable.type = Enums.ItemType.Consumable;

            if (consumable != null)
            {
                itemsDatabase.Add(consumable);

                Debug.Log("Loaded consumable item " + consumable.itemName + " and added to the dictionary.");
            }
        }

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
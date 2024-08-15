using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    [Header("Item Data JSON Files")]

    public TextAsset consumablesFile;

    private Dictionary<string, Item> itemsDatabase = new Dictionary<string, Item>();
    // Start is called before the first frame update
    void Start()
    {
        LoadItemsFromJson();
    }

    private void LoadItemsFromJson()
    {
        if(consumablesFile == null){
            Debug.LogError("JSON file was not assigned.");
            return;
        }

        string jsonString = consumablesFile.text;

        ConsumableData[] consumablesData = JsonUtility.FromJson<ConsumablesDataArray>(jsonString).consumables;

        foreach(ConsumableData data in consumablesData){
            CreateConsumable(data);
        }

    }

    private void CreateConsumable(ConsumableData data)
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

        if(consumable != null){
            itemsDatabase.Add(consumable.itemName, consumable);

            Debug.Log("Loaded consumable item " + consumable.itemName + " and added to the dictionary.");
        }

    }
}

[Serializable]
public class ConsumablesDataArray
{
    public ConsumableData[] consumables;
}

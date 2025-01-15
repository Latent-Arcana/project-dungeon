using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;

public class ContainerGeneration : MonoBehaviour
{
    List<Item> itemsDatabase;
    LootTable lootTable;

    public static ContainerGeneration Container_Generator { get; set; }

    // Singleton pattern
    void Awake()
    {
        // SINGLETON CHECK
        if (Container_Generator != null && Container_Generator != this)
        {
            Destroy(this);
        }
        else
        {
            Container_Generator = this;
        }
    }

    /// <summary>
    /// Initalized by Game Setup so that we can use the data loaded by ItemLoader safely without any dependencies between the two singletons
    /// </summary>
    /// <param name="initializationItems"></param>
    public void InitializeContainerGenerator(List<Item> initializationItems, LootTable initializationLoot)
    {

        itemsDatabase = initializationItems;
        lootTable = initializationLoot;

        return;
    }

    /// <summary>
    /// This is the function our ContainerBehavior script will use. The ContainerBehavior script is attached to any prefab
    /// that needs to be able to be opened. Enemies will also have this script on a child game object, so that we can spawn
    /// a body and place the "container" script on that
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="maxItemCount"></param>
    /// <returns></returns>

    public List<Item> GetItems(ObjectType objectType, int maxItemCount)
    {

        int itemCount = UnityEngine.Random.Range(1, maxItemCount + 1);

        List<Item> resultItems = new List<Item>();

        List<LootItem> loot = GetLootList(objectType);

        // IF THE LOOT LIST IS EMPTY, WE JUST SEND AN EMPTY LIST TO THE CONTAINER
        if (loot.Count != 0)
        {
            int currentCount = 0;

            while (currentCount < itemCount)
            {

                int lootRoll = UnityEngine.Random.Range(1, 101);

                foreach (LootItem lootItem in loot)
                {

                    // Debug.Log(lootItem.itemName + " has range (" + lootItem.minValue + "," + lootItem.maxValue + ") and loot roll is " + lootRoll);

                    if (lootItem.minValue <= lootRoll && lootItem.maxValue >= lootRoll)
                    {
                        Item item = itemsDatabase.Where(x => x.itemID == lootItem.itemID).First();

                       // Should remove duplicates so we don't have a bunch of the same thing every time 

                        if (!resultItems.Contains(item))
                        {
                            resultItems.Add(item); 
                        }

                        currentCount++;
                    }

                    else
                    {
                        // Debug.Log(lootRoll + " is not between (" + lootItem.minValue + "," + lootItem.maxValue + ")");
                    }
                }
            }
        }

        return resultItems;

    }


    /// <summary>
    /// This is where we define what kind of objects can have what kinds of items in them
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
    private List<Item> GetPossibleItems(ObjectType objectType)
    {

        List<Item> possibleItems = new List<Item>();

        // A chest can have anything that isn't a book or a special item
        if (objectType == ObjectType.Chest)
        {

            possibleItems = itemsDatabase.Where(x => x.type != ItemType.Book && x.type != ItemType.Special).ToList();

        }

        // A bookshelf can only have books
        else if (objectType == ObjectType.Bookshelf)
        {

            possibleItems = itemsDatabase.Where(x => x.type != ItemType.Book && x.type != ItemType.Special).ToList();

        }

        else if (objectType == ObjectType.Corpse)
        {
            possibleItems = itemsDatabase;
        }

        else
        {
            possibleItems = null;
        }

        return possibleItems;

    }

    /// <summary>
    /// This function returns a list of items that correspond to the rarity level relevant to the container being opened
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>

    private List<LootItem> GetLootList(ObjectType objectType)
    {
        float rarityCheck = UnityEngine.Random.value;

        switch (objectType)
        {
            case ObjectType.Chest:

                if (rarityCheck < .70f)
                {
                    return lootTable.commonLoot;
                }
                else if (rarityCheck >= .70f && rarityCheck < .90f)
                {
                    return new List<LootItem>();
                }

                else
                {
                    return lootTable.uncommonLoot;
                }


            case ObjectType.Corpse:

                if (rarityCheck < .20f)
                {
                    return new List<LootItem>();
                }
                else if (rarityCheck >= .20f && rarityCheck < .60f)
                {
                    return lootTable.commonLoot;
                }
                else if (rarityCheck >= .60f && rarityCheck < .90f)
                {
                    return lootTable.uncommonLoot;
                }
                else
                {
                    return lootTable.epicLoot;
                }


            default:
                return lootTable.commonLoot;
        }
    }


    public Item GetRandomItem()
    {

        int randomIndex = UnityEngine.Random.Range(0, itemsDatabase.Count);

        return itemsDatabase[randomIndex];
    }
}

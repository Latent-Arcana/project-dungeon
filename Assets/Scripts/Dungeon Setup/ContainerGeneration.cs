using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class ContainerGeneration : MonoBehaviour
{
    List<Item> itemsDatabase;

    public static ContainerGeneration Container_Generator { get; set; }

    // Singleton pattern
    void Awake()
    {
        // SINGLETON CHECK
        if (Container_Generator == null)
        {
            Container_Generator = this;
        }
        else if (Container_Generator != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(Container_Generator);
    }

    /// <summary>
    /// Initalized by Game Setup so that we can use the data loaded by ItemLoader safely without any dependencies between the two singletons
    /// </summary>
    /// <param name="initializationItems"></param>
    public void InitializeContainerGenerator(List<Item> initializationItems)
    {
        itemsDatabase = initializationItems;
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

        int itemCount = 0;

        List<Item> resultItems = new List<Item>();

        List<Item> possibleItems = GetPossibleItems(objectType);

        possibleItems.Shuffle(); // let's randomly shuffle our possible items!

        float dropItems = UnityEngine.Random.value;

        if (dropItems >= 0.5f)
        {
            // loop through each possible item and see if we can include that object
            foreach (Item possibleItem in possibleItems)
            {

                float dropChance = UnityEngine.Random.value; // returns float between 0 and 1

                if (dropChance >= 0.75f && itemCount <= maxItemCount)
                {
                    itemCount++;
                    resultItems.Add(possibleItem);
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


}

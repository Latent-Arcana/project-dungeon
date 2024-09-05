using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContainerGeneration;

public class EnemyCorpseBehavior : MonoBehaviour
{

    [SerializeField]
    public int maxItemCount; // this is where we're going to define how many items at a time can spawn in our object

    public List<Item> items;

    // We need to get container data for our corpse
    void Start()
    {
        items = Container_Generator.GetItems(Enums.ObjectType.Corpse, maxItemCount); // we just start this out as a corpse since we know it's not a chest or anything else
    }

    // TODO: INVENTORY INTERACTION
    public List<Item> Open(){
        return items;
    }

    public void RemoveItem(Item item){
        items.Remove(item);
    }

    void AddItem(Item item){
        items.Add(item);
    }


    // DEBUG
    public void ContainerDebugPrint()
    {

        Debug.Log("_________________________________________________");

        Debug.Log($"{gameObject.name} contains: ");

        foreach (Item item in items)
        {

            Debug.Log(item.itemName);
        }

        Debug.Log("_________________________________________________");
    }


}

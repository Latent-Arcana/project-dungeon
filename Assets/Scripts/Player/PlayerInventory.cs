using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Use inheritance so that our PlayerInventory and ContainerBehavior don't have so much code dupe between them
public class PlayerInventory : MonoBehaviour
{

    [SerializeField]
    public int maxItemCount; // this is where we're going to define how many items at a time can spawn in our object

    public List<Item> items;

    // TODO: INVENTORY INTERACTION
    public List<Item> Open(){
        return items;
    }

    void RemoveItem(Item item){
        items.Remove(item);
    }

    void AddItem(Item item){
        items.Add(item);
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

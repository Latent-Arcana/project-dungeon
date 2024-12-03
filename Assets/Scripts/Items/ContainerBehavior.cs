using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContainerGeneration;

public class ContainerBehavior : MonoBehaviour
{

    //Sprites
    [SerializeField]
    public Sprite spriteClosed;
    [SerializeField]
    public Sprite spriteOpenFull;
    [SerializeField]
    public Sprite spriteOpenEmpty;

    private SpriteRenderer spriteRenderer;


    [SerializeField]
    public int maxItemCount; // this is where we're going to define how many items at a time can spawn in our object

    public List<Item> items;
    private ObjectBehavior objectData;

    void Awake()
    {
        objectData = gameObject.GetComponent<ObjectBehavior>();
    }

    // We need to get container data for our container
    void Start()
    {
        items = Container_Generator.GetItems(objectData.ObjectType, maxItemCount);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void SwapSprite()
    {

        if (items.Count > 0)
        {
            spriteRenderer.sprite = spriteOpenFull;
        }
        else{
            spriteRenderer.sprite = spriteOpenEmpty;
        }
    }


    public List<Item> Open()
    {
        return items;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
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

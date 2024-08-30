using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{

    ////Objects////
    private UIDocument main_document;


    private VisualElement table;

    private List<VisualElement> rows;

    // private VisualElement row_1, row_4;

    public PlayerInventory playerInventoryBehavior;

    private List<Item> inventory;

    private void Awake()
    {

        main_document = this.GetComponent<UIDocument>();

        // row_1 = main_document.rootVisualElement.Q("Row1");
        // row_4 = main_document.rootVisualElement.Q("Row4");

        table = main_document.rootVisualElement.Q("Table");

        rows = table.Children().ToList();

        playerInventoryBehavior = GameObject.Find("Player").GetComponent<PlayerInventory>();

        inventory = playerInventoryBehavior.items;

        foreach (var row in rows)
        {
            //get all 10 buttons and assign them actions (delegates)
            //button += function
        }


        //testing
        InventoryRefresh();


        //Event Throw
        // Row4.Button += onDrop(row4)


        //Event Listeners
        //listen for Total Inventory Refresh on a change (such as use or drop)
        //listen for update to equip? - check boxes may need to be updated in UI


    }

    /// <summary>
    /// This populates the UI table with the current list of items
    /// Might change this to a Event Listener at some point
    /// </summary>
    private void InventoryRefresh() //List<Item> items
    {

        int i = 0;

        //loop through items table

        //foreach (Item item in items)
        for (i = 0; i < 10; i++)
        {

            if (i < inventory.Count)
            {
                //assign img
                Debug.Log(rows[i].Q("Icon").Children().First());
                //rows[i].Q("Icon").Children().First().style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/a.png"));

                //assign name
                Debug.Log(rows[i].Q("Name").Children().First());
                TextElement te = rows[i].Q("Name").Children().First() as TextElement;
                te.text = "word";


            }
            else
            {
                //assign img
                Debug.Log(rows[i].Q("Icon").Children().First());
                //rows[i].Q("Icon").Children().First().style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/a.png"));

                //assign name
                Debug.Log(rows[i].Q("Name").Children().First());
                TextElement te = rows[i].Q("Name").Children().First() as TextElement;
                te.text = "";

            }

            /*
            TODO: if Q performance is bad here, we can construct a List<CustomRowObject>  
            up in Awake to only load them once, something like:
            
            class row {
                TextElement Name,
                VisualElement image,
                TextElement stats
                etc
            }
            
            */


            //assign stats



            //assign equipped



            //i++;
        }
    }



}

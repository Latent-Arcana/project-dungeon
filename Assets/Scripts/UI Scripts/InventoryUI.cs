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
    private VisualElement parentContainer;

    private InputController input;


    private VisualElement table;

    private List<VisualElement> rows;

    // private VisualElement row_1, row_4;



    private void Awake()
    {

        input = GameObject.Find("InputController").GetComponent<InputController>();

        main_document = this.GetComponent<UIDocument>();

        parentContainer = main_document.rootVisualElement.Q("Container");

        table = main_document.rootVisualElement.Q("Table");

        rows = table.Children().ToList();




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

    private void OnEnable()
    {
        input.OnInventoryEnter += Event_OnInventoryEnter;

    }

    private void OnDisable()
    {
        input.OnInventoryEnter -= Event_OnInventoryEnter;
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

            //assign img
            Debug.Log(rows[i].Q("Icon").Children().First());
            //rows[i].Q("Icon").Children().First().style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/a.png"));

            //assign name
            Debug.Log(rows[i].Q("Name").Children().First());
            TextElement te = rows[i].Q("Name").Children().First() as TextElement;
            te.text = "word";

            //assign stats



            //assign equipped



            //i++;
        }
    }

    public void Event_OnInventoryEnter(object sender, EventArgs e)
    {
        input.ToggleMovement();
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public PlayerInventory playerInventoryBehavior;

    private List<Item> inventory;

    private void Awake()
    {

        input = GameObject.Find("InputController").GetComponent<InputController>();

        main_document = this.GetComponent<UIDocument>();

        parentContainer = main_document.rootVisualElement.Q("Container");

        table = main_document.rootVisualElement.Q("Table");

        rows = table.Children().ToList();

        playerInventoryBehavior = GameObject.Find("Player").GetComponent<PlayerInventory>();

        inventory = playerInventoryBehavior.items;

        int tempCounter = 0;

        foreach (var row in rows)
        {
            //get all 10 buttons and assign them actions (delegates)
            //button += function
            Button butt = row.Q("Drop").Children().First() as Button;

            int j = tempCounter;
            butt.clicked += () => DropItem(j);
            tempCounter++;

        }

        //Event Throw
        // Row4.Button += onDrop(row4)


        //Event Listeners
        //listen for update to equip? - check boxes may need to be updated in UI

    }


    void Update()
    {
        InventoryRefresh();
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

        inventory = playerInventoryBehavior.items;

        //loop through items table

        //foreach (Item item in items)
        for (i = 0; i < 10; i++)
        {

            if (i < inventory.Count)
            {

                rows[i].style.visibility = Visibility.Visible;

                //assign img
                Sprite sprt = Resources.Load<Sprite>(inventory[i].image);
                rows[i].Q("Icon").Children().First().style.backgroundImage = new StyleBackground(sprt);

                //assign name
                TextElement nameText = rows[i].Q("Name").Children().First() as TextElement;
                nameText.text = inventory[i].itemName;

                TextElement statsText = rows[i].Q("Stats").Children().First() as TextElement;
                statsText.text = inventory[i].statsText.ToString();


            }
            else
            {
                rows[i].style.visibility = Visibility.Hidden;

                //assign empty img ?
                //Sprite sprt = Resources.Load<Sprite>(inventory[i].image);
                rows[i].Q("Icon").Children().First().style.backgroundImage = null;

                TextElement nameText = rows[i].Q("Name").Children().First() as TextElement;
                nameText.text = "";

                TextElement statsText = rows[i].Q("Stats").Children().First() as TextElement;
                statsText.text = "";

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


    private void DropItem(int index)
    {
        Debug.Log(index);

        playerInventoryBehavior.RemoveItem(index);
    }

    public void Event_OnInventoryEnter(object sender, EventArgs e)
    {
        input.ToggleMovement();
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;

    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build.Player;
using UnityEditor.Search;
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
    private List<Toggle> equipmentToggles = new List<Toggle>();

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
            Toggle togg = row.Q("Equipped").Children().First() as Toggle;

            int toggleIndex = equipmentToggles.Count;

            togg.RegisterValueChangedCallback(evt =>
            {

                OnToggleValueChanged(evt.newValue, toggleIndex);

            });

            equipmentToggles.Add(togg); // there should be 10 of these


            butt.text = "drop";
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
        //EquipmentRefresh();
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

                // only show the EQUIP button if the item is equippable
                if (inventory[i].type != Enums.ItemType.Armor && inventory[i].type != Enums.ItemType.Weapon)
                {
                    equipmentToggles[i].visible = false;
                }

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

    private void OnToggleValueChanged(bool equipped, int index)
    {
        // first let's just figure out what we're even trying to check
        Item equippedItem = inventory[index];

        int currentArmor = playerInventoryBehavior.GetEquippedArmor();
        int currentWeapon = playerInventoryBehavior.GetEquippedWeapon();

        if (equipped == true)
        {
            if (equippedItem.type == Enums.ItemType.Armor)
            {
                // we can just straight up equip this
                if (currentArmor == -1)
                {
                    Equip(index);
                }

                else
                {
                    equipmentToggles[currentArmor].value = false; // unequip screen
                    Unequip(currentArmor); // unequip data

                    Equip(index);
                }

            }
            else if (equippedItem.type == Enums.ItemType.Weapon)
            {
                // we can just straight up equip this
                if (currentWeapon == -1)
                {
                    Equip(index);
                }

                else
                {
                    equipmentToggles[currentWeapon].value = false; // unequip screen
                    Unequip(currentWeapon); // unequip data

                    Equip(index);
                }

            }
        }

        else
        {
            Unequip(index);
        }



    }
    private void DropItem(int index)
    {
        Debug.Log(index);

        playerInventoryBehavior.RemoveItem(index);
    }

    private void Equip(int index)
    {
        playerInventoryBehavior.EquipItem(index);

    }

    private void Unequip(int index)
    {
        playerInventoryBehavior.HandleUnequip(index);
    }

    public void Event_OnInventoryEnter(object sender, EventArgs e)
    {
        input.ToggleMovement();
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;

    }


}

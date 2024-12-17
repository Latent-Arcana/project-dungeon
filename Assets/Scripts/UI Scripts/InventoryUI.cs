using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{



    //// UI Objects ////
    private UIDocument main_document;
    private VisualElement parentContainer;
    private VisualElement table;
    private List<VisualElement> rows;
    private List<Toggle> equipmentToggles = new List<Toggle>();


    //// Controllers /////
    private InputController input;


    //// Inventory /////
    public PlayerInventory playerInventoryBehavior;
    [SerializeField]
    public Inventory player_inventory;
    private List<Item> inventory;


    ///// Audio ////
    private MenuAudioController menuAudioController;



    private void Awake()
    {

        //controllers
        input = GameObject.Find("InputController").GetComponent<InputController>();

        //UI
        main_document = this.GetComponent<UIDocument>();
        parentContainer = main_document.rootVisualElement.Q("Container");
        table = main_document.rootVisualElement.Q("Table");
        rows = table.Children().ToList();

        //inventory
        playerInventoryBehavior = GameObject.Find("Player").GetComponent<PlayerInventory>();
        inventory = playerInventoryBehavior.inventory.items;

        //audio
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();



        //assign button/toggle callbacks
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

        inventory = playerInventoryBehavior.inventory.items;

        //loop through items table
        for (i = 0; i < 10; i++)
        {

            if (i < inventory.Count)
            {

                rows[i].style.visibility = Visibility.Visible;

                //assign img
                Sprite sprt = Resources.Load<Sprite>(inventory[i].image);
                rows[i].Q("Icon").Children().First().Children().First().style.backgroundImage = new StyleBackground(sprt);

                //assign name
                TextElement nameText = rows[i].Q("Name").Children().First() as TextElement;
                nameText.text = inventory[i].itemName;

                TextElement statsText = rows[i].Q("Stats").Children().First() as TextElement;
                statsText.text = inventory[i].statsText.ToString();

                //Debug.Log($"Comparing i ({i}) to armor ({player_inventory.equippedArmor}) and weapon ({player_inventory.equippedArmor})");

                if (i == player_inventory.equippedArmor || i == player_inventory.equippedWeapon)
                {
                    //Debug.Log($"Equipped item in slot {i}");
                    equipmentToggles[i].SetValueWithoutNotify(true);
                }

                else
                {
                    equipmentToggles[i].SetValueWithoutNotify(false);
                }

            }

            else //no inventory item to show in the row, so hide it
            {
                rows[i].style.visibility = Visibility.Hidden;

                rows[i].Q("Icon").Children().First().Children().First().style.backgroundImage = null;

                TextElement nameText = rows[i].Q("Name").Children().First() as TextElement;
                nameText.text = "";

                TextElement statsText = rows[i].Q("Stats").Children().First() as TextElement;
                statsText.text = "";

            }

        }
    }

    private void OnToggleValueChanged(bool equipped, int index)
    {

        // Before we do anything, we should check if this is a drop event. 
        // We don't want to pass the index in if we are just dropping this item
        if (index >= inventory.Count)
        {
            return;
        }
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

            else if (equippedItem.type == Enums.ItemType.Consumable)
            {
                Consume(index);
            }
        }

        else
        {
            Unequip(index);
        }

    }

    private void Consume(int index)
    {
        playerInventoryBehavior.Consume(index);

        // if the item was equipped, let's unequip it to update the screen
        if (equipmentToggles[index].value == true)
        {
            equipmentToggles[index].SetValueWithoutNotify(false);

        }

        playerInventoryBehavior.RemoveItem(index);
    }
    private void DropItem(int index)
    {

        menuAudioController.PlayAudioClip("ButtonClose");

        playerInventoryBehavior.HandleDropItemNarration(index);

        // if the item was equipped, let's unequip it to update the screen
        if (equipmentToggles[index].value == true)
        {
            
            equipmentToggles[index].SetValueWithoutNotify(false);

            playerInventoryBehavior.UnequipStatsChange(inventory[index]);
        }

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
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }


}

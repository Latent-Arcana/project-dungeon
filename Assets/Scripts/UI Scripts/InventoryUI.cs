using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public PlayerInventory playerInventory;
    [SerializeField]
    public Inventory inventory_class;
    private List<Item> inventory;

    // Durability Sprites //
    [SerializeField]
    Sprite dur5;
    [SerializeField]
    Sprite dur4;
    [SerializeField]
    Sprite dur3;
    [SerializeField]
    Sprite dur2;
    [SerializeField]
    Sprite dur1;


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
        playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();
        inventory = playerInventory.inventory.items;

        //audio
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();

        //by default, don't show the inventory (even if Artisan forgets to toggle the visibility off after she makes changes)
        parentContainer.style.display = DisplayStyle.None;

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

        inventory = playerInventory.inventory.items;

        //loop through items table
        for (i = 0; i < 10; i++)
        {

            if (i < inventory.Count)
            {

                rows[i].style.visibility = Visibility.Visible;

                // Heirarchy of the html:
                //Icon_Col
                //Icon_Bckgrnd
                //Durability
                //Icon

                //assign img for item
                Sprite sprt = Resources.Load<Sprite>(inventory[i].image);
                rows[i].Q("Icon").style.backgroundImage = new StyleBackground(sprt);

                //assign img for durability frame

                //get durability
                int durability = playerInventory.GetDurability(i);
                //int durability = 2;
                Sprite dur;

                //case 0-5
                switch (durability)
                {
                    case 5:
                        dur = dur5;
                        break;
                    case 4:
                        dur = dur4;
                        break;
                    case 3:
                        dur = dur3;
                        break;
                    case 2:
                        dur = dur2;
                        break;
                    case 1:
                        dur = dur1;
                        break;
                    default:
                        dur = null;
                        break;
                }

                rows[i].Q("Durability").style.backgroundImage = new StyleBackground(dur);

                //assign name
                TextElement nameText = rows[i].Q("Name").Children().First() as TextElement;
                nameText.text = inventory[i].itemName;

                TextElement statsText = rows[i].Q("Stats").Children().First() as TextElement;
                statsText.text = inventory[i].statsText.ToString();

                //Debug.Log($"Comparing i ({i}) to armor ({inventory_class.equippedArmor}) and weapon ({inventory_class.equippedArmor})");

                if (i == inventory_class.equippedArmor || i == inventory_class.equippedWeapon)
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

                //rows[i].Q("").Children().First().style.backgroundImage = null;

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


        int currentArmor = playerInventory.GetEquippedArmor();
        int currentWeapon = playerInventory.GetEquippedWeapon();

        if (equipped == true)
        {
            if (equippedItem.type == Enums.ItemType.Armor)
            {

                menuAudioController.PlayAudioClip("ArmorEquip");

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

                menuAudioController.PlayAudioClip("WeaponEquip");

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

            menuAudioController.PlayAudioClip("Unquip");

            Unequip(index);
        }

    }

    private void Consume(int index)
    {
        playerInventory.Consume(index);

        // if the item was equipped, let's unequip it to update the screen
        if (equipmentToggles[index].value == true)
        {
            equipmentToggles[index].SetValueWithoutNotify(false);

        }

        playerInventory.RemoveItem(index);
    }
    private void DropItem(int index)
    {

        menuAudioController.PlayAudioClip("ButtonClose");

        playerInventory.HandleDropItemNarration(index);

        // if the item was equipped, let's unequip it to update the screen
        if (equipmentToggles[index].value == true)
        {

            equipmentToggles[index].SetValueWithoutNotify(false);

            playerInventory.UnequipStatsChange(inventory[index]);
        }

        playerInventory.RemoveItem(index);
    }

    private void Equip(int index)
    {
        playerInventory.EquipItem(index);
    }

    private void Unequip(int index)
    {
        playerInventory.HandleUnequip(index);
    }

    public void Event_OnInventoryEnter(object sender, EventArgs e)
    {
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }


}

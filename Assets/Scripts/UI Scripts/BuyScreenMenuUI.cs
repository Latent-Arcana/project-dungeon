using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static StorePack;

public class BuyScreenMenuUI : MonoBehaviour
{
    private UIDocument main_document;

    private MainMenuUI mainMenuUI;
    private VisualElement buyScreenContainer, Row1, Row2, Row3;

    private Label cartosText;

    public int selected_pack = 0;

    public ExplorationData storeData;

    [SerializeField]
    public VisualTreeAsset template;

    private List<Pack> selectedPacks;


    ///// Audio ////
    private MenuAudioController menuAudioController;



    void Awake()
    {
        //// UI Document ////
        main_document = this.GetComponent<UIDocument>();
        mainMenuUI = this.GetComponent<MainMenuUI>();

        //// Containers ////
        buyScreenContainer = main_document.rootVisualElement.Q("BuyScreenContainer");
        cartosText = buyScreenContainer.Q("CartosGroup").Q("Cartos") as Label;

        Row1 = main_document.rootVisualElement.Q("Row1");
        Row2 = main_document.rootVisualElement.Q("Row2");
        Row3 = main_document.rootVisualElement.Q("Row3");

        storeData = SaveSystem.LoadPlayerSaveData();

        if (storeData != null)
        {
            cartosText.text = "Cartos: " + storeData.cartosEarned;
        }
        else
        {
            cartosText.text = "Cartos: 0";
        }


        //generate a list of 9 packs to be displayed.
        //Start with 5 default packs
        selectedPacks = new()
        {
            Store_Pack.GetNoPack(),
            Store_Pack.GetBasicWeaponPack(),
            Store_Pack.GetBasicArmorPack(),
            Store_Pack.GetAdvnacedWeaponPack(),
            Store_Pack.GetAdvancedArmorPack(),
        };

        //then add 4 random packs
        List<Pack> randomSelectedPacks = Store_Pack.GetRandomPacks();
        selectedPacks.AddRange(randomSelectedPacks);


        //TODO: maybe sort packs grid by price?


        //Create the pack objects from the template UI element
        for (int i = 0; i < 9; i++)
        {
            var tempElement = template.Instantiate().Children().FirstOrDefault();

            //Name Text
            TextElement tempText = tempElement.Q("PackName") as TextElement;
            tempText.text = selectedPacks[i].name;

            //Price Text
            tempText = tempElement.Q("CartoAmount") as TextElement;
            tempText.text = selectedPacks[i].price + " Cartos";

            //Assign click event
            Button tempButton = tempElement.Q("StoreButton") as Button;
            int tempIndex = i; //you have to create a new int or scope gets weird
            tempButton.clicked += () => HandlePack(tempIndex);

            //Add element to grid in correct row
            if (i >= 6)
            {
                Row3.Add(tempElement);
            }
            else if (i >= 3)
            {
                Row2.Add(tempElement);
            }
            else
            {
                Row1.Add(tempElement);
            }

        }


        // // Disable buttons if not enough Cartos
        // foreach (var pack in packCosts)
        // {
        //     if (storeData.cartosEarned < pack.Value)
        //     {
        //         DisableButton(pack.Key);
        //     }
        // }


    }

    public void HandlePack(int selectedPacksIndex)
    {

        //Get the info on the Pack you picked
        int cost = selectedPacks[selectedPacksIndex].price;
        selected_pack = selectedPacks[selectedPacksIndex].packId;

        //Debug.Log($"Selected Pack: {selected_pack}. Cost is: {cost}");

        //Spend your cartos
        storeData.cartosEarned -= cost;
        cartosText.text = "Cartos: " + storeData.cartosEarned;
        SaveSystem.SaveExplorationData(storeData);

        //Play the Game
        StartCoroutine(mainMenuUI.FadeScreenOnExit());

    }

    // public void DisableButton(Button button)
    // {
    //     button.SetEnabled(false); // Disables button interactions
    //     button.style.opacity = 0.25f; // Makes button appear greyed out
    //     button.AddToClassList("disabled-button");
    //     button.RemoveFromClassList("menu-pause-button:hover"); // Try forcing removal
    // }

}

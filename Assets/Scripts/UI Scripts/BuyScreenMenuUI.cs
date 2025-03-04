using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuyScreenMenuUI : MonoBehaviour
{
    private UIDocument main_document;

    private MainMenuUI mainMenuUI;
    private VisualElement buyScreenContainer;

    private Button noPack, basicWeaponPack, basicArmorPack, advancedWeaponPack, advancedArmorPack, alchemistPack, healerPack;

    private Label cartosText;

    public string selected_pack;

    public ExplorationData storeData;



    // Dictionary for pack costs
    private Dictionary<Button, int> packCosts;

    void Awake()
    {
        //// UI Document ////
        main_document = this.GetComponent<UIDocument>();

        mainMenuUI = this.GetComponent<MainMenuUI>();

        //// Containers ////
        buyScreenContainer = main_document.rootVisualElement.Q("BuyScreenContainer");

        cartosText = buyScreenContainer.Q("CartosGroup").Q("Cartos") as Label;


        storeData = SaveSystem.LoadPlayerSaveData();

        if (storeData != null)
        {
            cartosText.text = "Cartos: " + storeData.cartosEarned;
        }
        else
        {
            cartosText.text = "Cartos: 0";
        }

        noPack = buyScreenContainer.Q("NoPack") as Button;
        basicWeaponPack = buyScreenContainer.Q("BasicWeaponPack") as Button;
        basicArmorPack = buyScreenContainer.Q("BasicArmorPack") as Button;
        advancedWeaponPack = buyScreenContainer.Q("AdvancedWeaponPack") as Button;
        advancedArmorPack = buyScreenContainer.Q("AdvancedArmorPack") as Button;
        alchemistPack = buyScreenContainer.Q("AlchemistPack") as Button;
        healerPack = buyScreenContainer.Q("HealerPack") as Button;


        // Initialize pack costs
        packCosts = new Dictionary<Button, int>
        {
            { noPack, 0},
            { basicWeaponPack, 20 },
            { basicArmorPack, 30 },
            { advancedWeaponPack, 40 },
            { advancedArmorPack, 50 },
            { alchemistPack, 60 },
            { healerPack, 70 }
        };

        // Disable buttons if not enough Cartos
        foreach (var pack in packCosts)
        {
            if (storeData.cartosEarned < pack.Value)
            {
                DisableButton(pack.Key);
            }
        }


        // Assign click events
        foreach (var pack in packCosts)
        {
            pack.Key.clicked += () => HandlePack(pack.Key);
        }

    }

    public void HandlePack(Button packButton)
    {

        int cost;

        if (packCosts.ContainsKey(packButton))
        {
            cost = packCosts[packButton];
        }
        else
        {
            cost = 0;
            Debug.Log($"BuyScreenMenuUI, a Pack Button was accessed that does not exist.");
        }

        selected_pack = packButton.name;
        
        Debug.Log($"Selected Pack: {selected_pack}. Cost is: {cost}");

        storeData.cartosEarned -= cost;

        cartosText.text = "Cartos: " + storeData.cartosEarned;

        SaveSystem.SaveExplorationData(storeData);

        StartCoroutine(mainMenuUI.FadeScreenOnExit());

    }

    public void DisableButton(Button button)
    {
        button.SetEnabled(false); // Disables button interactions
        button.style.opacity = 0.25f; // Makes button appear greyed out
        button.AddToClassList("disabled-button");
        button.RemoveFromClassList("menu-pause-button:hover"); // Try forcing removal
    }

}

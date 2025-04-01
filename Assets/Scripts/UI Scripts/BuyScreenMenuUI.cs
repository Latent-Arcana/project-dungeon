using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BuyScreenMenuUI : MonoBehaviour
{
    private UIDocument main_document;

    private MainMenuUI mainMenuUI;
    private VisualElement buyScreenContainer, Row1, Row2, Row3;

    //private Button noPack, basicWeaponPack, basicArmorPack, advancedWeaponPack, advancedArmorPack, alchemistPack, healerPack;

    private Label cartosText;

    public string selected_pack;

    public ExplorationData storeData;

    [SerializeField]
    public VisualTreeAsset template;


    ///// Audio ////
    private MenuAudioController menuAudioController;


    public enum Packs
    {
        noPack,
        basicWeaponPack,
        basicArmorPack,
        advancedWeaponPack,
        advancedArmorPack,
        alchemistPack,
        healerPack,

        //I didn't implement these yet:
        strengthPack,
        AgilityPack,
        SpeedPack,
        hpPack,
        apPack


    }

    // Dictionary for pack costs
    private Dictionary<Packs, int> packCosts;

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


        // Initialize pack costs
        packCosts = new Dictionary<Packs, int>
        {
            { Packs.noPack, 0},
            { Packs.basicWeaponPack, 20 },
            { Packs.basicArmorPack, 30 },
            { Packs.advancedWeaponPack, 40 },
            { Packs.advancedArmorPack, 50 },
            { Packs.alchemistPack, 60 },
            { Packs.healerPack, 70 }
        };

        Dictionary<Packs, string> packNames = new Dictionary<Packs, string>
        {
            { Packs.noPack, "No Pack"},
            { Packs.basicWeaponPack, "Basic Weapon Pack" },
            { Packs.basicArmorPack, "Basic Armor Pack" },
            { Packs.advancedWeaponPack, "Advanced Weapon Pack" },
            { Packs.advancedArmorPack, "Advanced Armor Pack" },
            { Packs.alchemistPack, "Alchemist Pack" },
            { Packs.healerPack, "Healer Pack" }
        };

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
        List<Packs> selectedPacks = new List<Packs>{

            //row 1
            Packs.noPack,
            Packs.basicWeaponPack,
            Packs.basicArmorPack,

            //row 2
            Packs.alchemistPack,
            Packs.advancedWeaponPack,
            Packs.advancedArmorPack,

            //row 3 (maybe do some random selection here instead of hard coding them)
            Packs.healerPack,
            Packs.healerPack,
            Packs.healerPack
        };



        for (int i = 0; i < 9; i++)
        {
            var tempElement = template.Instantiate().Children().FirstOrDefault();

            //TODO the assignment stuff here
            TextElement tempText = tempElement.Q("PackName") as TextElement;
            tempText.text = packNames[selectedPacks[i]].ToString();

            tempText = tempElement.Q("CartoAmount") as TextElement;
            tempText.text = packCosts[selectedPacks[i]].ToString() + " Cartos";


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

        // noPack = buyScreenContainer.Q("NoPack") as Button;
        // basicWeaponPack = buyScreenContainer.Q("BasicWeaponPack") as Button;
        // basicArmorPack = buyScreenContainer.Q("BasicArmorPack") as Button;
        // advancedWeaponPack = buyScreenContainer.Q("AdvancedWeaponPack") as Button;
        // advancedArmorPack = buyScreenContainer.Q("AdvancedArmorPack") as Button;
        // alchemistPack = buyScreenContainer.Q("AlchemistPack") as Button;
        // healerPack = buyScreenContainer.Q("HealerPack") as Button;




        // // Disable buttons if not enough Cartos
        // foreach (var pack in packCosts)
        // {
        //     if (storeData.cartosEarned < pack.Value)
        //     {
        //         DisableButton(pack.Key);
        //     }
        // }


        // // Assign click events
        // foreach (var pack in packCosts)
        // {
        //     pack.Key.clicked += () => HandlePack(pack.Key);
        // }

    }

    public void HandlePack(Packs packButton)
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

        selected_pack = packButton.ToString();

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

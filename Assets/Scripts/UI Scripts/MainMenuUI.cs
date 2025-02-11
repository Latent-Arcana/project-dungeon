using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.Mathematics;



public class MainMenuUI : MonoBehaviour
{
    ////Objects////
    private UIDocument main_document;
    private OptionsMenuUI optionsMenuUI;

    ////Buttons////
    // Main Container Buttons
    private Button PlayButton;
    private Button OptionsButton;
    private Button HelpButton;
    private Button CreditsButton;
    private Button QuitButton;

    //Help Container Buttons
    private Button BackButton_help;

    // End of Game Container Buttons
    private Button QuitButton_EndGame;

    //Credits Container Buttons
    private Button BackButton_credits;


    ////Containers////
    private VisualElement optionsContainer;
    private VisualElement mainContainer;
    private VisualElement parentContainer;
    private VisualElement helpContainer;
    private VisualElement creditsContainer;
    private VisualElement endContainer;
    private VisualElement quitContainer;

    // END GAME
    public bool gameCompleted = false; //debug

    //Audio
    [SerializeField]
    private AudioMixer audioMixer;
    private MenuAudioController menuAudioController;


    // Screen Fade
    float fadeInDuration = 1f;
    float fadeOutDuration = 1f;
    VisualElement screenOverlay;
    float screenFadeElapsedTime = 0f;
    bool screenFadeCompleted = false;
    bool beginTransition = false;


    // STATS UI
    VisualElement statsOnLeft;
    VisualElement statsOnRight;
    VisualElement seedGroup;


    // Initializing a new BSP when clicking Play Game
    [SerializeField]
    public PlayerStatsManager Player_Stats;
    [SerializeField]
    public Inventory Player_Inventory;


    private void Awake()
    {
        //UI Document
        main_document = this.GetComponent<UIDocument>();

        //Audio
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();

        //Options menu class script
        optionsMenuUI = this.GetComponent<OptionsMenuUI>();

        ////Buttons////  

        // Main Container Buttons
        PlayButton = main_document.rootVisualElement.Q("PlayButton") as Button;
        OptionsButton = main_document.rootVisualElement.Q("OptionsButton") as Button;
        CreditsButton = main_document.rootVisualElement.Q("CreditsButton") as Button;
        HelpButton = main_document.rootVisualElement.Q("HelpButton") as Button;
        QuitButton = main_document.rootVisualElement.Q("QuitButton") as Button;

        //Help Container Buttons
        BackButton_help = main_document.rootVisualElement.Q("BackButtonHelp") as Button;

        //Credits Container Buttons
        BackButton_credits = main_document.rootVisualElement.Q("BackButtonCredits") as Button;

        // End of Game Container Buttons
        QuitButton_EndGame = main_document.rootVisualElement.Q("QuitGameFromEnd") as Button;

        //// Containers ////
        optionsContainer = main_document.rootVisualElement.Q("OptionsContainer");
        mainContainer = main_document.rootVisualElement.Q("MainContainer");
        parentContainer = main_document.rootVisualElement.Q("Container");
        helpContainer = main_document.rootVisualElement.Q("PlayingTheGame");
        creditsContainer = main_document.rootVisualElement.Q("CreditsScreen");

        // Stats 
        statsOnLeft = main_document.rootVisualElement.Q("StatsOnLeft");
        statsOnRight = main_document.rootVisualElement.Q("StatsOnRight");
        seedGroup = main_document.rootVisualElement.Q("SeedGroup");

        //Make sure that we have just the main container showing
        mainContainer.style.display = DisplayStyle.Flex;
        optionsContainer.style.display = DisplayStyle.None;
        helpContainer.style.display = DisplayStyle.None;
        creditsContainer.style.display = DisplayStyle.None;

        //Don't show the easter egg by default
        endContainer = main_document.rootVisualElement.Q("YouBeatTheGame");
        endContainer.style.display = DisplayStyle.None;

        //Add actions to buttons
        PlayButton.clicked += PlayGame;
        OptionsButton.clicked += optionsMenuUI.GoToOptions;
        BackButton_help.clicked += HelpMenu;
        BackButton_credits.clicked += CreditsMenu;
        HelpButton.clicked += HelpMenu;
        CreditsButton.clicked += CreditsMenu;
        QuitButton.clicked += QuitGame;
        QuitButton_EndGame.clicked += QuitGame;


        // LETS FADE IN THE SCREEN ON AWAKE AT THE MAIN MENU
        screenOverlay = main_document.rootVisualElement.Q("ScreenOverlay");
        screenOverlay.style.opacity = 1f;

        // LETS CHECK OUR STATS TO SEE OUR PLAYER's PROGRESS
        ExplorationData expData = SaveSystem.LoadPlayerSaveData();

        Label dungeonsMapped = statsOnLeft.Q("DungeonsMapped") as Label;
        Label roomsMapped = statsOnLeft.Q("RoomsMapped") as Label;
        Label enemiesKilled = statsOnLeft.Q("EnemiesKilled") as Label;

        Label dungeonsVisited = statsOnRight.Q("DungeonsVisited") as Label;
        Label cartographersLost = statsOnRight.Q("CartographersLost") as Label;
        Label completionPercentage = statsOnRight.Q("CompletionPercentage") as Label;

        // DISPLAY STATS IF WE HAVE THEM
        if (expData != null)
        {
            dungeonsMapped.text = "Dungeons Mapped: " + expData.dungeonsFullyMapped.ToString();
            roomsMapped.text = "Rooms Mapped: " + expData.roomsMappedSuccessfully.ToString();
            enemiesKilled.text = "Enemies Killed: " + expData.enemiesKilled.ToString();
            dungeonsVisited.text = "Dungeons Visited: " + expData.dungeonsVisited.ToString();
            cartographersLost.text = "Cartographers Lost: " + expData.cartographersLost.ToString();
            completionPercentage.text = "Completion: " + ((expData.mappedDungeons.Count / 1000.0f) * 100f).ToString("0.00") + "%";

            // IF THE GAME IS OVER, WE JUST CUT TO THE END GAME SCREEN

            if (expData.mappedDungeons.Count >= 1000 || gameCompleted)
            {
                //Show the end game container instead of the Main one
                endContainer.style.display = DisplayStyle.Flex;
                mainContainer.style.display = DisplayStyle.None;

                //assign stats
                Label dungeonsMapped_End = endContainer.Q("End_DungeonsMapped") as Label;
                Label roomsMapped_End = endContainer.Q("End_RoomsMapped") as Label;
                Label enemiesKilled_End = endContainer.Q("End_EnemiesKilled") as Label;
                Label dungeonsVisited_End = endContainer.Q("End_DungeonsVisited") as Label;
                Label cartographersLost_End = endContainer.Q("End_CartographersLost") as Label;
                Label completionPercentage_End = endContainer.Q("End_CompletionPercentage") as Label;

                dungeonsMapped_End.text = "Dungeons Mapped: " + expData.dungeonsFullyMapped.ToString();
                roomsMapped_End.text = "Rooms Mapped: " + expData.roomsMappedSuccessfully.ToString();
                enemiesKilled_End.text = "Enemies Killed: " + expData.enemiesKilled.ToString();
                dungeonsVisited_End.text = "Dungeons Visited: " + expData.dungeonsVisited.ToString();
                cartographersLost_End.text = "Cartographers Lost: " + expData.cartographersLost.ToString();
                completionPercentage_End.text = "Completion: " + ((expData.mappedDungeons.Count / 1000.0f) * 100f).ToString("0.00") + "%";

            }
        }
        // NO STATS, WE CAN JUST DISPLAY NOTHING
        else
        {
            dungeonsMapped.text = "";
            roomsMapped.text = "";
            enemiesKilled.text = "";
            dungeonsVisited.text = "";
            cartographersLost.text = "";
            completionPercentage.text = "";
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" && !screenFadeCompleted)
        {
            FadeScreenOnStart();
        }
    }

    private void FadeScreenOnStart()
    {
        screenFadeElapsedTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(screenFadeElapsedTime / fadeInDuration);

        // Gradually fade out the overlay
        screenOverlay.style.opacity = Mathf.Lerp(1, 0, normalizedTime);

        // Check if the fade is completed
        if (normalizedTime >= 1f)
        {
            screenFadeCompleted = true;
            screenFadeElapsedTime = 0f; // Reset for the next effect
            screenOverlay.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator FadeScreenOnExit()
    {
        screenOverlay.style.display = DisplayStyle.Flex; // Ensure the overlay is visible before fading out
        float fadeTime = 0f;
        while (fadeTime < fadeOutDuration)
        {
            fadeTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(fadeTime / fadeOutDuration);

            // Gradually fade in the overlay
            screenOverlay.style.opacity = Mathf.Lerp(0, 1, normalizedTime);

            yield return null;

        }

        Player_Stats.Initialize(); // Resetting the player's stats to base stats when a new game begins
        Player_Inventory.Reset(); // Resetting the player's inventory and equipment when a new game begins
        SceneManager.LoadScene("BSP");
    }

    private void HelpMenu()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        ToggleHelp();
    }

    private void ToggleHelp()
    {
        mainContainer.style.display = (mainContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
        helpContainer.style.display = (helpContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void CreditsMenu()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        ToggleCredits();
    }

    private void ToggleCredits()
    {
        mainContainer.style.display = (mainContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
        creditsContainer.style.display = (creditsContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    //load the game (if main menu), or unpause the game (if pause menu)
    private void PlayGame()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        StartCoroutine(FadeScreenOnExit());
    }

    public void QuitGame()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        Debug.Log("Quit Game");
        Application.Quit();
    }

}

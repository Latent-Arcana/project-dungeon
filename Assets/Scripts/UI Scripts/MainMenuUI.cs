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
    private InputController input;

    ////Buttons////
    private Button PlayButton;
    private Button QuitButton;
    private Button OptionsButton;
    private Button HelpButton;
    private Button BackButton;
    private Button BackButton_help;

    ////Containers////
    private VisualElement optionsContainer;
    private VisualElement mainContainer;
    private VisualElement parentContainer;
    private VisualElement helpContainer;

    //// Options
    private SaveOptions ops;
    private Slider volMusicSlider;
    private Slider volEffectsSlider;
    private Slider ambientSlider;
    //private DropdownField screenResDropdown;
    private Toggle fullScreenToggle;

    // END GAME
    public bool gameCompleted = false;

    //Audio
    [SerializeField]
    private AudioMixer audioMixer;


    // Screen Fade
    float fadeInDuration = 1f;
    float fadeOutDuration = 1f;
    VisualElement screenOverlay;
    float screenFadeElapsedTime = 0f;
    bool screenFadeCompleted = false;
    bool beginTransition = false;

    private MenuAudioController menuAudioController;

    // STATS UI
    VisualElement statsOnLeft;
    VisualElement statsOnRight;


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

        ////Buttons////  
        //similar to getting an HTML element by #ID
        PlayButton = main_document.rootVisualElement.Q("PlayButton") as Button;
        QuitButton = main_document.rootVisualElement.Q("QuitButton") as Button;
        OptionsButton = main_document.rootVisualElement.Q("OptionsButton") as Button;
        BackButton = main_document.rootVisualElement.Q("BackButton") as Button;
        BackButton_help = main_document.rootVisualElement.Q("BackButtonHelp") as Button;
        HelpButton = main_document.rootVisualElement.Q("HelpButton") as Button;

        //// Containers ////
        optionsContainer = main_document.rootVisualElement.Q("OptionsContainer");
        mainContainer = main_document.rootVisualElement.Q("MainContainer");
        parentContainer = main_document.rootVisualElement.Q("Container");
        helpContainer = main_document.rootVisualElement.Q("PlayingTheGame");

        statsOnLeft = main_document.rootVisualElement.Q("StatsOnLeft");
        statsOnRight = main_document.rootVisualElement.Q("StatsOnRight");

        screenOverlay = main_document.rootVisualElement.Q("ScreenOverlay");

        // Debug.Log($"Main Container is {mainContainer}");

        // Debug.Log($"Help Container is {helpContainer}");

        //Make sure that we have just the main container showing
        mainContainer.style.display = DisplayStyle.Flex;
        optionsContainer.style.display = DisplayStyle.None;
        helpContainer.style.display = DisplayStyle.None;

        //// Options ////
        ops = SaveSystem.LoadOptions();
        volMusicSlider = main_document.rootVisualElement.Q("VolumeMusicSlider") as Slider;
        volEffectsSlider = main_document.rootVisualElement.Q("VolumeSoundEffectsSlider") as Slider;
        ambientSlider = main_document.rootVisualElement.Q("VolumeAmbientSlider") as Slider;
        //screenResDropdown = main_document.rootVisualElement.Q("ResolutionDropdown") as DropdownField;
        fullScreenToggle = main_document.rootVisualElement.Q("FullScreen") as Toggle;

        ////Events////

        //Buttons
        PlayButton.clicked += PlayGame;
        QuitButton.clicked += QuitGame;
        OptionsButton.clicked += GoToOptions;
        BackButton.clicked += SaveSettings;
        BackButton_help.clicked += HelpMenu;
        HelpButton.clicked += HelpMenu;

        //Sliders
        volMusicSlider.RegisterCallback<ChangeEvent<float>>(SetMusicVolume);
        volEffectsSlider.RegisterCallback<ChangeEvent<float>>(SetSoundEffectsVolume);
        ambientSlider.RegisterCallback<ChangeEvent<float>>(SetAmbientVolume);


        //Dropdowns
        //screenResDropdown.RegisterCallback<ChangeEvent<string>>(OnScreenResolutionChanged);

        //Toggles
        fullScreenToggle.RegisterCallback<ChangeEvent<bool>>(SetFullScreen);

        if (SceneManager.GetActiveScene().name != "Main Menu")
        {

            //Pause Menu not displayed by default on gameplay
            parentContainer.style.display = DisplayStyle.None;

            //player only exists in gameplay
            input = GameObject.Find("InputController").GetComponent<InputController>();
        }

        // IF WE'RE AT THE MAIN MENU WE HAVE SOME THINGS TO WORK OUT
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {

            // LETS FADE IN THE SCREEN ON AWAKE AT THE MAIN MENU
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
                completionPercentage.text = "Completion: " + ((expData.mappedDungeons.Count / 10000.0f) * 100f).ToString("0.00") + "%";

                // IF THE GAME IS OVER, WE JUST CUT TO THE END GAME SCREEN

                if (expData.mappedDungeons.Count >= 10000)
                {
                    gameCompleted = true;

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
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            input.OnMenuEnter += Event_OnMenuEnter;
        }


    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            input.OnMenuEnter -= Event_OnMenuEnter;
        }

    }

    private void Start()
    {

        //warning, don't put audiomixer setfloat in MonoBehavior.Awake()
        audioMixer.SetFloat("MixerSFXVolume", ConvertVolumeToDb(ops.soundEffectVolume));
        audioMixer.SetFloat("MixerMusicVolume", ConvertVolumeToDb(ops.musicVolume));
        audioMixer.SetFloat("MixerAmbientVolume", ConvertVolumeToDb(ops.ambientVolume));

        // Screen.SetResolution(ops.screenOptions.screenWidth, ops.screenOptions.screenHeight, ops.screenOptions.fullScreen);

        SaveSystem.PrintPlayerSaveData();

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



    //Volume Settings
    private void SetMusicVolume(ChangeEvent<float> ev)
    {
        ops.musicVolume = volMusicSlider.value;
        audioMixer.SetFloat("MixerMusicVolume", ConvertVolumeToDb(ops.musicVolume));
    }

    private void SetSoundEffectsVolume(ChangeEvent<float> ev)
    {
        ops.soundEffectVolume = volEffectsSlider.value;
        audioMixer.SetFloat("MixerSFXVolume", ConvertVolumeToDb(ops.soundEffectVolume));
    }

    private void SetAmbientVolume(ChangeEvent<float> ev)
    {
        ops.ambientVolume = ambientSlider.value;
        audioMixer.SetFloat("MixerAmbientVolume", ConvertVolumeToDb(ops.ambientVolume));
    }

    private float ConvertVolumeToDb(float vol)
    {
        //log scale because decibels
        //the sliders are set to .0001f to 1
        return Mathf.Log10(vol) * 20;
    }

    private void SaveSettings()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        SaveSystem.SaveOptions(ops);
        ToggleOptions();
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

    private void OnScreenResolutionChanged(ChangeEvent<string> evt)
    {
        switch (evt.newValue)
        {
            case "1920x1080":
                ops.screenOptions = new ScreenOptions(1920, 1080, Screen.fullScreen);
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;

            case "640x480":
                ops.screenOptions = new ScreenOptions(640, 480, Screen.fullScreen);
                Screen.SetResolution(640, 480, Screen.fullScreen);
                break;

            default:
                ops.screenOptions = new ScreenOptions(640, 480, Screen.fullScreen);
                Screen.SetResolution(640, 480, Screen.fullScreen);
                break;
        }
    }

    private void SetFullScreen(ChangeEvent<bool> evt)
    {
        ops.screenOptions.fullScreen = evt.newValue;
        Screen.SetResolution(Screen.width, Screen.height, evt.newValue);
    }


    //Options
    private void GoToOptions()
    {
        menuAudioController.PlayAudioClip("ButtonOpen");

        //set sliders to the saved values
        volMusicSlider.SetValueWithoutNotify(ops.musicVolume);
        volEffectsSlider.SetValueWithoutNotify(ops.soundEffectVolume);
        ambientSlider.SetValueWithoutNotify(ops.ambientVolume);

        //string resString = ops.screenOptions.screenWidth.ToString() + "x" + ops.screenOptions.screenHeight.ToString();
        //screenResDropdown.SetValueWithoutNotify(resString);
        fullScreenToggle.SetValueWithoutNotify(ops.screenOptions.fullScreen);

        //show options menu
        ToggleOptions();
    }

    private void ToggleOptions()
    {
        mainContainer.style.display = (mainContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
        optionsContainer.style.display = (optionsContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }


    //Pause Menu
    private void TogglePauseMenu()
    {
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    //load the game (if main menu), or unpause the game (if pause menu)
    private void PlayGame()
    {
        if (gameCompleted)
        {
            SceneManager.LoadScene("CompletedGame");
        }
        else if (SceneManager.GetActiveScene().name == "Main Menu")
        {

            menuAudioController.PlayAudioClip("ButtonClose");

            StartCoroutine(FadeScreenOnExit());
        }
        else //Pause Menu Resume Button
        {
            menuAudioController.PlayAudioClip("ButtonClose");

            TogglePauseMenu();
            input.currentInputState = InputController.InputState.Gameplay; //this is the only place we change the state of the menus outside of input controller, so we manually set the state
            input.ToggleMovement();
        }

    }

    public void QuitGame()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        Debug.Log("Quit Game");
        Application.Quit();
    }

    /// <summary>
    /// Caught in MainMenuUI, Thrown by InputController
    /// </summary>
    public void Event_OnMenuEnter(object sender, EventArgs e)
    {
        TogglePauseMenu();
    }

}

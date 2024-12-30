using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;



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


    //Audio
    [SerializeField]
    private AudioMixer audioMixer;

    private MenuAudioController menuAudioController;


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
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {

            menuAudioController.PlayAudioClip("ButtonClose");

            Player_Stats.Initialize(); // Resetting the player's stats to base stats when a new game begins
            Player_Inventory.Reset(); // Resetting the player's inventory and equipment when a new game begins
            SceneManager.LoadScene("BSP");
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

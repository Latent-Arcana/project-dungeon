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
    private Button BackButton;

    ////Containers////
    private VisualElement optionsContainer;
    private VisualElement mainContainer;
    private VisualElement parentContainer;

    //// Options
    private SaveOptions ops;
    private Slider volMusicSlider;
    private Slider volEffectsSlider;

    //Audio
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioClip audioOpen;
    [SerializeField]
    private AudioClip audioClose;
    private AudioSource menuAudio;

    private void Awake()
    {
        //UI Document
        main_document = this.GetComponent<UIDocument>();

        //Audio
        menuAudio = GetComponentInChildren<AudioSource>();

        ////Buttons////  
        //similar to getting an HTML element by #ID
        PlayButton = main_document.rootVisualElement.Q("PlayButton") as Button;
        QuitButton = main_document.rootVisualElement.Q("QuitButton") as Button;
        OptionsButton = main_document.rootVisualElement.Q("OptionsButton") as Button;
        BackButton = main_document.rootVisualElement.Q("BackButton") as Button;

        //// Containers ////
        optionsContainer = main_document.rootVisualElement.Q("OptionsContainer");
        mainContainer = main_document.rootVisualElement.Q("MainContainer");
        parentContainer = main_document.rootVisualElement.Q("Container");

        //Shouldn't have to init these but they were null otherwise
        mainContainer.style.display = DisplayStyle.Flex;
        optionsContainer.style.display = DisplayStyle.None;

        //// Options ////
        ops = SaveSystem.LoadOptions();
        volMusicSlider = main_document.rootVisualElement.Q("VolumeMusicSlider") as Slider;
        volEffectsSlider = main_document.rootVisualElement.Q("VolumeSoundEffectsSlider") as Slider;

        ////Events////

        //Buttons
        PlayButton.clicked += PlayGame;
        QuitButton.clicked += QuitGame;
        OptionsButton.clicked += GoToOptions;
        BackButton.clicked += SaveSettings;

        //Sliders
        volMusicSlider.RegisterCallback<ChangeEvent<float>>(SetMusicVolume);
        volEffectsSlider.RegisterCallback<ChangeEvent<float>>(SetSoundEffectsVolume);

        
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            //Pause Menu not displayed by default on gameplay
            parentContainer.style.display = DisplayStyle.None;

            //player only exists in gameplay
            input = GameObject.Find("InputController").GetComponent<InputController>();
        }

    }

    private void Start()
    {
        //warning, don't put audiomixer setfloat in MonoBehavior.Awake()
        audioMixer.SetFloat("MixerSFXVolume", ConvertVolumeToDb(ops.soundEffectVolume));
        audioMixer.SetFloat("MixerMusicVolume", ConvertVolumeToDb(ops.musicVolume));
    }


    private void Update()
    {
        //Listen for the trigger to pause the game, if not on the Main menu screen
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Main Menu")
        {
            TogglePauseMenu();
        }
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

    private float ConvertVolumeToDb(float vol)
    {
        //log scale because decibels
        //the sliders are set to .0001f to 1
        return Mathf.Log10(vol) * 20;
    }

    private void SaveSettings()
    {
        PlayAudioClose();
        SaveSystem.SaveOptions(ops);
        ToggleOptions();
    }


    //Options
    private void GoToOptions()
    {
        PlayAudioOpen();
        volMusicSlider.SetValueWithoutNotify(ops.musicVolume);
        volEffectsSlider.SetValueWithoutNotify(ops.soundEffectVolume);
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
        input.ToggleMovement();
        input.TogglePauseMenu();
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }



    //Play and Quit Buttons
    private void PlayGame()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            PlayAudioOpen();
            SceneManager.LoadScene("BSP");
        }
        else
        {
            PlayAudioClose();
            TogglePauseMenu();
        }

    }

    public void QuitGame()
    {
        PlayAudioClose();
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void PlayAudioOpen()
    {
        menuAudio.clip = audioOpen;
        menuAudio.Play();
    }


    private void PlayAudioClose()
    {
        menuAudio.clip = audioClose;
        menuAudio.Play();
    }

}

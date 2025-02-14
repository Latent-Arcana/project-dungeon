using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class OptionsMenuUI : MonoBehaviour
{

    private UIDocument main_document;

    private VisualElement optionsContainer;
    private VisualElement mainContainer;

    private Button BackButton_options;

    //// Options
    private SaveOptions ops;
    private Slider volMusicSlider;
    private Slider volEffectsSlider;
    private Slider ambientSlider;
    private Toggle fullScreenToggle;


    //Audio
    [SerializeField]
    private AudioMixer audioMixer;
    private MenuAudioController menuAudioController;


    private void Awake()
    {

        //UI Document
        main_document = this.GetComponent<UIDocument>();

        //// Containers ////
        optionsContainer = main_document.rootVisualElement.Q("OptionsContainer");
        mainContainer = main_document.rootVisualElement.Q("MainContainer");

        //Audio to play button sounds
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();

        //Pull the current saved options
        ops = SaveSystem.LoadOptions();

        //Find the UI elements
        volMusicSlider = main_document.rootVisualElement.Q("VolumeMusicSlider") as Slider;
        volEffectsSlider = main_document.rootVisualElement.Q("VolumeSoundEffectsSlider") as Slider;
        ambientSlider = main_document.rootVisualElement.Q("VolumeAmbientSlider") as Slider;
        fullScreenToggle = main_document.rootVisualElement.Q("FullScreen") as Toggle;
        BackButton_options = optionsContainer.Q("BackButtonOptions") as Button;

        //Assign actions to Sliders
        volMusicSlider.RegisterCallback<ChangeEvent<float>>(SetMusicVolume);
        volEffectsSlider.RegisterCallback<ChangeEvent<float>>(SetSoundEffectsVolume);
        ambientSlider.RegisterCallback<ChangeEvent<float>>(SetAmbientVolume);

        //Assign actions to Toggles
        fullScreenToggle.RegisterCallback<ChangeEvent<bool>>(SetFullScreen);

        //Assign actions to Back Button
        BackButton_options.clicked += SaveSettings;

    }

    private void Start()
    {
        //warning, don't put audiomixer setfloat in MonoBehavior.Awake()
        audioMixer.SetFloat("MixerSFXVolume", ConvertVolumeToDb(ops.soundEffectVolume));
        audioMixer.SetFloat("MixerMusicVolume", ConvertVolumeToDb(ops.musicVolume));
        audioMixer.SetFloat("MixerAmbientVolume", ConvertVolumeToDb(ops.ambientVolume));

        Screen.fullScreen = ops.screenOptions.fullScreen;
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

    public void SaveSettings()
    {
        //if we're in the pause menu, we need to set the state in input controller.
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            GameObject.Find("InputController").GetComponent<InputController>().currentInputState = InputController.InputState.PauseMenu;
        }

        //Button Sound
        menuAudioController.PlayAudioClip("ButtonClose");

        //Save the data to file
        SaveSystem.SaveOptions(ops);

        //Toggle the UI
        ToggleOptions();
    }

    private void SetFullScreen(ChangeEvent<bool> evt)
    {
        ops.screenOptions.fullScreen = evt.newValue;
        Screen.fullScreen = evt.newValue;

         // Ensure the fullscreen mode is properly set
        if (evt.newValue)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; // Or Windowed
        }
    }

    //Options
    public void GoToOptions()
    {
        menuAudioController.PlayAudioClip("ButtonOpen");

        //set sliders to the saved values
        volMusicSlider.SetValueWithoutNotify(ops.musicVolume);
        volEffectsSlider.SetValueWithoutNotify(ops.soundEffectVolume);
        ambientSlider.SetValueWithoutNotify(ops.ambientVolume);

        //set full screen toggle
        fullScreenToggle.SetValueWithoutNotify(ops.screenOptions.fullScreen);

        //if we're in the pause menu, we need to set the state in input controller.
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            GameObject.Find("InputController").GetComponent<InputController>().currentInputState = InputController.InputState.PauseMenu_Sub;
        }

        //show options menu
        ToggleOptions();
    }

    private void ToggleOptions()
    {
        mainContainer.style.display = (mainContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
        optionsContainer.style.display = (optionsContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;

    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SplashScreenUIController : MonoBehaviour
{


    private UIDocument main_document;

    public Label anyKeyText;
    public VisualElement screenOverlay;

    public bool fadingInAnyKeyText = true;
    public bool beginTransition = false;
    public bool screenFadeCompleted = false;
    public float elapsedTime = 0f;
    public float fadeTextDuration = 2.5f;

    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1.5f;

    private BackgroundMusicController backgroundMusicController;

    //audio stuff
    private SaveOptions ops;
    [SerializeField]
    private AudioMixer audioMixer;

    void Awake()
    {
        main_document = this.GetComponent<UIDocument>();

        anyKeyText = main_document.rootVisualElement.Q("AnyKeyText") as Label;

        screenOverlay = main_document.rootVisualElement.Q("ScreenOverlay");
        screenOverlay.style.opacity = 1;

        ops = SaveSystem.LoadOptions();

    }

    private void Start()
    {
        backgroundMusicController = GameObject.Find("BackgroundAudio").GetComponent<BackgroundMusicController>();
        backgroundMusicController.ChangeSongForScene("Main Menu");

        //do NOT try and do this in awake
        audioMixer.SetFloat("MixerMusicVolume", ConvertVolumeToDb(ops.musicVolume));

    }

    void Update()
    {
        if (!screenFadeCompleted)
        {
            FadeScreenOnStart();
        }
        else
        {
            FadeAnyKeyText();

            if (Input.anyKeyDown && beginTransition == false)
            {
                beginTransition = true;
                StartCoroutine(FadeScreenOnExit());
            }
        }

    }

    private void FadeScreenOnStart()
    {
        elapsedTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(elapsedTime / fadeInDuration);

        // Gradually fade out the overlay
        screenOverlay.style.opacity = Mathf.Lerp(1, 0, normalizedTime);

        // Check if the fade is completed
        if (normalizedTime >= 1f)
        {
            screenFadeCompleted = true;
            elapsedTime = 0f; // Reset for the next effect
        }
    }

    private void FadeAnyKeyText()
    {
        elapsedTime += Time.deltaTime;
        float normalizedTime = (elapsedTime % fadeTextDuration) / fadeTextDuration;

        // Calculate alpha based on whether fading in or out
        float alpha = fadingInAnyKeyText ? Mathf.Lerp(0, 1, normalizedTime) : Mathf.Lerp(1, 0, normalizedTime);

        // Switch fading direction at the halfway point
        if (normalizedTime >= 0.5f && fadingInAnyKeyText) fadingInAnyKeyText = false;
        if (normalizedTime < 0.5f && !fadingInAnyKeyText) fadingInAnyKeyText = true;

        // Apply the opacity
        anyKeyText.style.opacity = alpha;
    }

    private IEnumerator FadeScreenOnExit()
    {
        float fadeTime = 0f;
        while (fadeTime < fadeOutDuration)
        {
            fadeTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(fadeTime / fadeOutDuration);

            // Gradually fade in the overlay
            screenOverlay.style.opacity = Mathf.Lerp(0, 1, normalizedTime);

            yield return null;
        }

        SceneManager.LoadScene("Main Menu");

    }

    private float ConvertVolumeToDb(float vol)
    {
        return Mathf.Log10(vol) * 20;
    }
}

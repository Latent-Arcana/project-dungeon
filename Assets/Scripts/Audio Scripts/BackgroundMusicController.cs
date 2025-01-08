using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.Mathematics;
using System;

public class BackgroundMusicController : MonoBehaviour
{


    //Audio Objects
    [SerializeField]
    private AudioMixer audioMixer;
    private AudioSource backgroundAudio;

    //Clips
    [SerializeField]
    private AudioClip mainMenuMusic;

    [SerializeField]
    private List<AudioClip> levelMusic;

    [SerializeField]
    private AudioClip gameOverMusic;

    //Sound numbers
    float currentMixerVolume, currentMixerVolumeDb;
    float fadeOutDuration = 5f;
    float fadeInDuration = 4f;
    float volumeOriginalSetting = 0f;


    Coroutine fadeOutProcess;

    //Scene
    private Scene currentScene;

    bool beginFadeOutAudio = false;

    void Awake()
    {
        backgroundAudio = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        ObjectGeneration.RoomComplete += UpdateAudioLevelDuringLoad;
        ObjectGeneration.AllRoomsPlacementComplete += SwapAudioOnBSPLoad;
    }

    void OnDisable()
    {
        ObjectGeneration.RoomComplete -= UpdateAudioLevelDuringLoad;
        ObjectGeneration.AllRoomsPlacementComplete -= SwapAudioOnBSPLoad;
    }

    private void SwapAudioOnBSPLoad()
    {
        StopCoroutine(fadeOutProcess);
        //Debug.Log("Stopped coroutine. Volume set to: " + volumeOriginalSetting);
        backgroundAudio.volume = volumeOriginalSetting;
        ChangeSongForScene("BSP");

        beginFadeOutAudio = false;
    }

    public void StopAudio()
    {
        backgroundAudio.Stop();
    }

    public void ChangeSongForScene(string newSceneName)
    {
        //stop current song
        backgroundAudio.Stop();

        //change audio clip song
        if (newSceneName == "Main Menu")
        {
            backgroundAudio.clip = mainMenuMusic;
        }
        else if (newSceneName == "GameOver")
        {
            backgroundAudio.clip = gameOverMusic;
        }
        else if (newSceneName == "BSP")
        {
            backgroundAudio.clip = GetRandomLevelTrack();
        }

        //start playing new audio clip
        StartCoroutine(FadeInAudio());
    }

    void UpdateAudioLevelDuringLoad(float percentage)
    {
        if (percentage >= .80f && beginFadeOutAudio == false)
        {
            //Debug.Log("setting volume OG to: " + backgroundAudio.volume);
            beginFadeOutAudio = true;
            volumeOriginalSetting = backgroundAudio.volume;
            fadeOutProcess = StartCoroutine(FadeOutAudio());
        }
    }

    private IEnumerator FadeOutAudio()
    {
        //backgroundAudio.volume = 0f;
        //backgroundAudio.Play();

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            backgroundAudio.volume = Mathf.Lerp(volumeOriginalSetting, 0f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundAudio.volume = 0f; // Ensure volume is fully set to 0 at the end

    }

    private IEnumerator FadeInAudio()
    {
        float volumeStartOnFadeIn = backgroundAudio.volume;
        backgroundAudio.volume = 0;
        backgroundAudio.Play();
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            backgroundAudio.volume = Mathf.Lerp(0, volumeStartOnFadeIn, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        backgroundAudio.volume = volumeStartOnFadeIn; // Ensure volume is fully set to the original at the end

    }



    // ////StartCoroutine(StartFade(newSceneName));
    // private IEnumerator StartFade(string newSceneName)
    // {

    //     backgroundAudio.Stop();

    //     //change audio clip song
    //     if (newSceneName == "Main Menu")
    //     {
    //         backgroundAudio.clip = mainMenuMusic;
    //     }
    //     // else if (newSceneName == "BSP")
    //     // {
    //     //     backgroundAudio.clip = levelMusic;
    //     // }
    //     else if (newSceneName == "GameOver")
    //     {
    //         backgroundAudio.clip = gameOverMusic;
    //     }
    //     else if (newSceneName == "Loading")
    //     {
    //         backgroundAudio.clip = GetRandomLevelTrack();
    //     }

    //     //start playing new audio clip
    //     // except for BSP, which should already be playing from Loading
    //     if (newSceneName != "BSP")
    //     {
    //         backgroundAudio.Play();
    //     }

    //     //temp, set audio back to good volume
    //     audioMixer.SetFloat("MixerMusicVolume", currentMixerVolumeDb);

    //     yield break;
    // }


    /// <summary>
    /// Converts a Volume (0-100) into decibels
    /// </summary>
    /// <param name="vol">A volume ranging 0 to 100 percent </param>
    /// <returns>A volume in decibels</returns>
    private float ConvertVolumeToDb(float vol)
    {
        return Mathf.Log10(vol) * 20;
    }

    /// <summary>
    /// Converts a Volume in decibels to a volume percent (0-100) 
    /// </summary>
    /// <param name="vol">A volume in decibels</param>
    /// <returns>A volume percent (0-100) </returns>
    private float ConvertVolumeToPercent(float vol)
    {
        return Mathf.Pow(10, vol / 20);
    }

    private AudioClip GetRandomLevelTrack()
    {
        int a = UnityEngine.Random.Range(0, levelMusic.Count);
        return levelMusic[a];
    }

}

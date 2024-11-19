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
    float fadeOutDuration = .1f;
    float fadeInDuration = 1f;

    //Scene
    private Scene currentScene;


    void Awake()
    {
        backgroundAudio = GetComponent<AudioSource>();

    }


    public void ChangeSongForScene(string newSceneName)
    {
        //save the current volume level so we don't have to load it from the save ops
        //audioMixer.GetFloat("MixerMusicVolume", out currentMixerVolumeDb);

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
        else if (newSceneName == "Loading")
        {
            backgroundAudio.clip = GetRandomLevelTrack();
        }

        //start playing new audio clip
        // except for BSP, which should already be playing from Loading
        if (newSceneName != "BSP")
        {
            backgroundAudio.Play();
        }

        //temp, set audio back to good volume
        //audioMixer.SetFloat("MixerMusicVolume", currentMixerVolumeDb);
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

    private AudioClip GetRandomLevelTrack(){
        int a = UnityEngine.Random.Range(0,levelMusic.Count);
        return levelMusic[a];
    }

}

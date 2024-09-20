using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.Mathematics;

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
    private AudioClip levelMusic;
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
        audioMixer.GetFloat("MixerMusicVolume", out currentMixerVolumeDb);

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
            backgroundAudio.clip = levelMusic;
        }

        //start playing new audio clip
        // except for BSP, which should already be playing from Loading
        if (newSceneName != "BSP")
        {
            backgroundAudio.Play();
        }

        //temp, set audio back to good volume
        audioMixer.SetFloat("MixerMusicVolume", currentMixerVolumeDb);
    }



    ////StartCoroutine(StartFade(newSceneName));
    private IEnumerator StartFade(string newSceneName)
    {




        // float currentTime = 0;
        // float currentVol;

        // // // fade OUT
        // audioMixer.GetFloat("MixerMusicVolume", out currentMixerVolumeDb); //store to fade back in later

        // //get the current volume setting and convert to non-decimal
        // audioMixer.GetFloat("MixerMusicVolume", out currentVol);
        // currentVol = Mathf.Pow(10, currentVol / 20);

        // float targetValue = Mathf.Clamp(0, 0.0001f, 1);


        // Debug.Log($"Fading from {currentVol} to {targetValue}");

        // while (currentTime < fadeOutDuration)
        // {
        //     currentTime += Time.deltaTime;
        //     float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / fadeOutDuration);

        //     Debug.Log($"New Volume {newVol} at time {currentTime}");
        //     audioMixer.SetFloat("MixerMusicVolume", Mathf.Log10(newVol) * 20);
        //     yield return null;
        // }

        backgroundAudio.Stop();

        //change audio clip song
        if (newSceneName == "Main Menu")
        {
            backgroundAudio.clip = mainMenuMusic;
        }
        // else if (newSceneName == "BSP")
        // {
        //     backgroundAudio.clip = levelMusic;
        // }
        else if (newSceneName == "GameOver")
        {
            backgroundAudio.clip = gameOverMusic;
        }
        else if (newSceneName == "Loading")
        {
            backgroundAudio.clip = levelMusic;
        }

        //start playing new audio clip
        // except for BSP, which should already be playing from Loading
        if (newSceneName != "BSP")
        {
            backgroundAudio.Play();
        }

        //temp, set audio back to good volume
        audioMixer.SetFloat("MixerMusicVolume", currentMixerVolumeDb);

        // //fade back to previously set volume

        // currentTime = 0;
        // audioMixer.GetFloat("MixerMusicVolume", out currentVol);

        // currentVol = Mathf.Pow(10, currentVol / 20);
        // targetValue = Mathf.Clamp(ConvertVolumeToPercent(currentMixerVolumeDb), 0.0001f, 1);

        // Debug.Log($"Fading from {currentVol} to {targetValue}");

        // while (currentTime < fadeInDuration)
        // {
        //     currentTime += Time.deltaTime;
        //     float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / fadeInDuration);

        //     Debug.Log($"New Volume {newVol} at time {currentTime}");

        //     audioMixer.SetFloat("MixerMusicVolume", Mathf.Log10(newVol) * 20);
        //     yield return null;
        // }

        yield break;
    }


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

}

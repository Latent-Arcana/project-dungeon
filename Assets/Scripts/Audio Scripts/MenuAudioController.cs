using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuAudioController : MonoBehaviour
{
    //Audio Objects
    [SerializeField]
    private AudioMixer audioMixer;

    private AudioSource menuAudioSource;

    //Clips
    [SerializeField]
    private AudioClip audioClip_ButtonOpen;

    [SerializeField]
    private AudioClip audioClip_ButtonClose;

    [SerializeField]
    private AudioClip audioClip_SelectSlider; //might tie audio to clicking on the vol slider, not sure yet

    [SerializeField]
    private AudioClip audioClip_PlayGame;

    void Awake()
    {
        menuAudioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(string audioClip) //TODO: make this an enum
    {
        switch (audioClip)
        {
            case "ButtonOpen":
                menuAudioSource.clip = audioClip_ButtonOpen;
                break;

            case "ButtonClose":
                menuAudioSource.clip = audioClip_ButtonClose;
                break;

            case "SelectSlider":
                menuAudioSource.clip = audioClip_SelectSlider;
                break;

            case "PlayGame":
                menuAudioSource.clip = audioClip_PlayGame;
                break;

            default:
                Debug.Log("Unknown Audio called in Menu Audio Controller");
                menuAudioSource.clip = null;
                break;
        }

        menuAudioSource.Play();
    }

}

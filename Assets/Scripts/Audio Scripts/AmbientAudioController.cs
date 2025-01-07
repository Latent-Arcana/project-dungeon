using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientAudioController : MonoBehaviour
{

    [SerializeField]
    AudioMixer audioMixer;

    [SerializeField]
    AudioSource ambientAudioSource;

    [SerializeField]
    AudioClip audioClip_Portal;

    [SerializeField]
    AudioClip audioClip_Chest;

    [SerializeField]
    AudioClip audioClip_Rest;

    [SerializeField]
    AudioClip audioClip_Bookshelf;


    [SerializeField]
    AudioClip audioClip_Bless;
    void Awake()
    {
        ambientAudioSource = GetComponent<AudioSource>();
    }


    public void PlayAudioClip(string clipName)
    {
        switch (clipName)
        {
            case "Portal":
                ambientAudioSource.clip = audioClip_Portal;
                break;
            case "Bless":
                ambientAudioSource.clip = audioClip_Bless;
                break;
            case "Chest":
                ambientAudioSource.clip = audioClip_Chest;
                break;
            case "Rest":
                ambientAudioSource.clip = audioClip_Rest;
                break;
            case "Bookshelf":
                ambientAudioSource.clip = audioClip_Bookshelf;
                break;
            default:
                return;
        }

        ambientAudioSource.Play();
    }

}

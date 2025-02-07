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
    AudioSource damageAudioSource;

    [SerializeField]
    AudioClip audioClip_Portal;

    [SerializeField]
    AudioClip audioClip_Chest;

    [SerializeField]
    AudioClip audioClip_Rest;

    [SerializeField]
    AudioClip audioClip_Bookshelf;


    [SerializeField]
    AudioClip audioClip_Damage;


    [SerializeField]
    AudioClip audioClip_PortalSpawn;

    [SerializeField]
    AudioClip audioClip_Bless;
    void Awake()
    {

        ambientAudioSource = GameObject.Find("AmbientAudio").GetComponent<AudioSource>();
        damageAudioSource = GameObject.Find("DamageAudio").GetComponent<AudioSource>();
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
            case "PortalSpawn":
                ambientAudioSource.clip = audioClip_PortalSpawn;
                break;
            default:
                return;
        }

        ambientAudioSource.Play();
    }

    public void PlayDamageAudio(string clipName)
    {
        switch (clipName)
        {
            case "Damage":
                damageAudioSource.clip = audioClip_Damage;
                break;
            default:
                return;
        }

        damageAudioSource.Play();
    }

}

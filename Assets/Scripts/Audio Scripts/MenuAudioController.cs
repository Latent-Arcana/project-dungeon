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

    [SerializeField]
    private AudioClip audioClip_ScrollOpen;

    [SerializeField]
    private AudioClip audioClip_ScrollClose;

    [SerializeField]
    private AudioClip audioClip_EquipWeapon;

    [SerializeField]
    private AudioClip audioClip_EquipArmor;

    [SerializeField]
    private AudioClip audioClip_Unquip;

    [SerializeField]
    private AudioClip audioClip_Consume;

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

            case "MapOpen":
                menuAudioSource.clip = audioClip_ScrollOpen;
                break;

            case "MapClose":
                menuAudioSource.clip = audioClip_ScrollClose;
                break;

            case "SelectSlider":
                menuAudioSource.clip = audioClip_SelectSlider;
                break;

            case "PlayGame":
                //menuAudioSource.clip = audioClip_PlayGame;
                // PLAY NO AUDIO ON THIS CLICK, JUST GO TO THE GAME - HTH
                break;

            case "Portal":
                menuAudioSource.clip = audioClip_PlayGame;
                break;

            case "WeaponEquip":
                menuAudioSource.clip = audioClip_EquipWeapon;
                break;

            case "Unquip":
                menuAudioSource.clip = audioClip_Unquip;
                break;

            case "ArmorEquip":
                menuAudioSource.clip = audioClip_EquipArmor;
                break;

            case "Consume":
                menuAudioSource.clip = audioClip_Consume;
                break;

            default:
                Debug.Log("Unknown Audio called in Menu Audio Controller");
                menuAudioSource.clip = null;
                break;
        }

        menuAudioSource.Play();
    }

}

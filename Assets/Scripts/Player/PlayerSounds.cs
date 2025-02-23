using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    AudioSource PlayerAudioSource;

    //These could be arrays if we want a set of sounds to play
    [SerializeField]
    AudioClip ouch;
    [SerializeField]
    AudioClip punch;
    [SerializeField]
    AudioClip damage;

    [SerializeField]
    AudioClip ohhhhNoooooIDiiiiiied;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAudioSource = GetComponent<AudioSource>();
    }
    public void PlayOuchSound(){
        PlayerAudioSource.clip = ouch;
        PlayerAudioSource.Play();
    }

    public void PlayPunchSound(){
        PlayerAudioSource.clip = punch;
        PlayerAudioSource.Play();
    }

    public void PlayDiedSound(){
        PlayerAudioSource.clip = ohhhhNoooooIDiiiiiied;
        PlayerAudioSource.Play();
    }

    public void PlayDamageSound(){
        PlayerAudioSource.clip = damage;
        PlayerAudioSource.Play();
    }
}

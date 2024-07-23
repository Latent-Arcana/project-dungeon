using System.Collections;
using System.Collections.Generic;
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
    AudioClip ohhhhNoooooIDiiiiiied;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // //TODO: This is only for testing, put this somewhere else where the action is happening
        // if (Input.GetKeyUp(KeyCode.UpArrow))
        // {
        //     PlayOuchSound();
        // }
        // else if (Input.GetKeyUp(KeyCode.DownArrow)){
        //     PlayPunchSound();
        // }
        // else if(Input.GetKeyUp(KeyCode.D)){
        //     PlayDiedSound();
        // }
    }

    //TODO: These will cut off whatever audio is already playing
    // So we might want to add a buffer


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
}

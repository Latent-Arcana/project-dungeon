using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveOptions
{
// This structure exists to pass into the binary/json serializer for save data
    public float musicVolume;
    public float soundEffectVolume;
    public float ambientVolume;

    //constructor used to load from file into a SaveOptions game object
    public SaveOptions(SaveOptions options){
        musicVolume = options.musicVolume;
        soundEffectVolume = options.soundEffectVolume;
        ambientVolume = options.ambientVolume;
    }

    //constructor used to initialize the SaveOptions game object otherwise
    public SaveOptions (float mVolume, float sfxVolume, float ambVolume){
        musicVolume = mVolume;
        soundEffectVolume = sfxVolume;
        ambientVolume = ambVolume;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SplashScreenUIController : MonoBehaviour
{


    private UIDocument main_document;

    private BackgroundMusicController backgroundMusicController;

    //audio stuff
    private SaveOptions ops;
    [SerializeField]
    private AudioMixer audioMixer;

    void Awake()
    {
        main_document = this.GetComponent<UIDocument>();

        ops = SaveSystem.LoadOptions();

    }

    private void Start()
    {
        backgroundMusicController = GameObject.Find("BackgroundAudio").GetComponent<BackgroundMusicController>();
        backgroundMusicController.ChangeSongForScene("Main Menu");

        //do NOT try and do this in awake
        audioMixer.SetFloat("MixerMusicVolume", ConvertVolumeToDb(ops.musicVolume));

    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    private float ConvertVolumeToDb(float vol)
    {
        return Mathf.Log10(vol) * 20;
    }
}

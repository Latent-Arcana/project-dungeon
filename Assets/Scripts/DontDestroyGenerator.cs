using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyGenerator : MonoBehaviour
{

    [SerializeField]
    GameObject mainMenuAudio;

    [SerializeField]
    GameObject gameScoreController;


    void Awake()
    {

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {

            //check for existing object with name
            GameObject objs = GameObject.Find("Audio");

            //if the object doesnt exist, create it (then never destroy it)
            if (objs == null)
            {
                GameObject obj = Instantiate(mainMenuAudio);
                obj.name = "Audio";
            }


        }

        else if (SceneManager.GetActiveScene().name == "BSP")
        {

            GameObject objs = GameObject.Find("GameStats");

            if (objs == null)
            {
                GameObject obj = Instantiate(gameScoreController);
                obj.name = "GameStats";
            }
        }

    }


}

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

  

            //check for existing object with name
            GameObject objs = GameObject.Find("Audio");

            //if the object doesnt exist, create it (then never destroy it)
            if (objs == null)
            {
                GameObject obj = Instantiate(mainMenuAudio);
                obj.name = "Audio";
            }


      

        if (SceneManager.GetActiveScene().name == "BSP")
        {

            //GameObject audioObj = ;

            if (GameObject.Find("GameStats") == null)
            {
                GameObject obj = Instantiate(gameScoreController);
                obj.name = "GameStats";
            }
        }

    }


}

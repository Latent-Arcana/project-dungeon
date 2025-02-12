using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;

public class LoadingScreenUI : MonoBehaviour
{
    //// UI Document ////
    private UIDocument loading_document;

    //// Container ////
    private VisualElement loadingContainer;

    //// Text ////
    private TextElement loadingText;
    float progressValue = 0f;

    // Scene Stuff
    private AsyncOperation loadingOperation;


    int progressPrintCount = 0;

    void Awake()
    {
        loading_document = this.GetComponent<UIDocument>();
        //laoding screen should be enabled by default, on the highest UI layer

        loadingText = loading_document.rootVisualElement.Q("LoadingText") as TextElement;
        loadingContainer = loading_document.rootVisualElement.Q("Container");

        loadingContainer.style.display = DisplayStyle.Flex;

    }

    void OnEnable()
    {
        ObjectGeneration.RoomComplete += SingleRoomCompleted;
    }

    void OnDisable()
    {
        ObjectGeneration.RoomComplete -= SingleRoomCompleted;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "BSP")
        {
            loadingOperation = SceneManager.LoadSceneAsync("BSP");
        }
    }

    void Update()
    {

        if (SceneManager.GetActiveScene().name != "BSP")
        {
            progressValue = loadingOperation.progress / 0.9f;
            loadingText.text += "\nGenerating the dungeon: " + progressValue * 100 + "%";
        }
    }

    private void SingleRoomCompleted(float percentage)
    {
        if (progressPrintCount <= 25 || progressPrintCount % 3 == 0)
        {
            loadingText.text += "\nGenerating rooms: " + String.Format("{0:F0}%", percentage * 100);
        }
        ++progressPrintCount;
    }



    // public void DisableLoadingScreen()
    // {
    //     loadingContainer.style.display = DisplayStyle.None;
    // }

    // public void EnableLoadingScreen()
    // {
    //     loadingContainer.style.display = DisplayStyle.Flex;
    // }

}
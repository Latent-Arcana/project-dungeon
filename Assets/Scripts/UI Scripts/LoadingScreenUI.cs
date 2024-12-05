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
    private VisualElement loadingAnimation;

    //// Text ////
    private TextElement loadingText;
    float progressValue = 0f;

    // Scene Stuff
    private AsyncOperation loadingOperation;

    public Sprite[] animationFrames;
    int animationTick = 0;
    int spriteIndex = 0;

    void Awake()
    {
        loading_document = this.GetComponent<UIDocument>();
        //laoding screen should be enabled by default, on the highest UI layer

        loadingText = loading_document.rootVisualElement.Q("LoadingText") as TextElement;
        loadingAnimation = loading_document.rootVisualElement.Q("LoadingAnimation") as VisualElement;
        loadingContainer = loading_document.rootVisualElement.Q("Container");

        loadingContainer.style.display = DisplayStyle.Flex;

    }

    void OnEnable(){
        ObjectGeneration.RoomComplete += SingleRoomCompleted;
    }

    void OnDisable(){
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
        ++animationTick;

        if(animationTick % 45 == 0){

            loadingAnimation.style.backgroundImage = null;

            loadingAnimation.style.backgroundImage = animationFrames[spriteIndex].texture;

            spriteIndex++;

            if(spriteIndex >= animationFrames.Length){
                spriteIndex = 0;
            }

        } 

        if (SceneManager.GetActiveScene().name != "BSP")
        {
            progressValue = loadingOperation.progress / 0.9f;
            loadingText.text = "Generating the dungeon: " + progressValue * 100 + "%";
        }
    }

    private void SingleRoomCompleted(float percentage){

        loadingText.text = "Generating rooms: " + String.Format("{0:F0}%", percentage * 100);

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
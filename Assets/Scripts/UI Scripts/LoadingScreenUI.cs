using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        loading_document = this.GetComponent<UIDocument>();
        //laoding screen should be enabled by default, on the highest UI layer

        loadingText = loading_document.rootVisualElement.Q("LoadingText") as TextElement;
        loadingContainer = loading_document.rootVisualElement.Q("Container");

        loadingContainer.style.display = DisplayStyle.Flex;

    }

    void Start()
    {
        loadingOperation = SceneManager.LoadSceneAsync("BSP");
    }

    void Update()
    {
        progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        loadingText.text = "Loading: " + Mathf.Round(progressValue * 100) + "%";
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameOverUI : MonoBehaviour
{
    //// UI Document ////
    private UIDocument main_document;

    //// Buttons ////
    private Button MainMenuButton;
    private Button QuitButton;
    private Button NewGameButton;

    //// Text ////
    private TextElement statsText1;
    private TextElement statsText2;
    private TextElement killedByText;
    private GameStats gameStats;

    //// Music ////
    private BackgroundMusicController backgroundMusicController;
    private MenuAudioController menuAudioController;


    private void Awake()
    {
        //UI Document
        main_document = this.GetComponent<UIDocument>();

        //Text
        statsText1 = main_document.rootVisualElement.Q("StatsText") as TextElement;
        statsText2 = main_document.rootVisualElement.Q("StatsText_2") as TextElement;

        killedByText = main_document.rootVisualElement.Q("KilledByText") as TextElement;
        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();

        ////Buttons////  
        //similar to getting an HTML element by #ID
        MainMenuButton = main_document.rootVisualElement.Q("MainMenuButton") as Button;
        QuitButton = main_document.rootVisualElement.Q("QuitButton") as Button;
        NewGameButton = main_document.rootVisualElement.Q("NewGameButton") as Button;

        // Button Events //
        MainMenuButton.clicked += ReturnToMenu;
        QuitButton.clicked += QuitGame;
        NewGameButton.clicked += StartNewGame;

        // Audio
        backgroundMusicController = GameObject.Find("BackgroundAudio").GetComponent<BackgroundMusicController>();
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();

    }

    void Start()
    {
        SetGameStatText();
    }

    private void SetGameStatText()
    {
        int num = gameStats.GetNumerator();
        int denom = gameStats.GetDenominator();
        int roomsVisited = gameStats.GetRoomsVisited(); //currently not being incremented anywhere

        killedByText.text = gameStats.GetDeathText();

        statsText1.text = @$"Total rooms marked correctly: {num}/{denom}";
        statsText2.text = @$"Total rooms visited: {roomsVisited}";
    }


    public void QuitGame()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        backgroundMusicController.ChangeSongForScene("Main Menu");
        gameStats.NewGame();
        SceneManager.LoadScene("Main Menu");
    }

    public void StartNewGame()
    {
        menuAudioController.PlayAudioClip("PlayGame");
     //   backgroundMusicController.ChangeSongForScene("Loading");
        gameStats.NewGame();
        SceneManager.LoadScene("BSP");
    }
}

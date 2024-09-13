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
    private TextElement statsText;
    private GameStats gameStats;

    //// Music ////
    private BackgroundMusicController backgroundMusicController;


    private void Awake()
    {
        //UI Document
        main_document = this.GetComponent<UIDocument>();

        //Text
        statsText = main_document.rootVisualElement.Q("StatsText") as TextElement;
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


    }

    private void SetGameStatText()
    {
        int num = gameStats.GetNumerator();
        int denom = gameStats.GetDenominator();
        int roomsVisited = gameStats.GetRoomsVisited(); //currently not set in GameStats

        statsText.text = @$"Final Stats: \n
        Total rooms marked correctly: {num}/{denom}\n
        Total rooms visited: {roomsVisited}\n
        ";
    }


    public void QuitGame()
    {
        //PlayAudioClose();
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        backgroundMusicController.ChangeSongForScene("Main Menu");
        gameStats.NewGame();
        SceneManager.LoadScene("Main Menu");
    }

    public void StartNewGame()
    {
        backgroundMusicController.ChangeSongForScene("BSP");
        gameStats.NewGame();
        SceneManager.LoadScene("Loading");
    }
}

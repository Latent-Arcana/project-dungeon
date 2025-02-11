using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using Unity.VisualScripting;

public class PauseMenuUI : MonoBehaviour
{

    private UIDocument main_document;
    private InputController inputControls;
    private OptionsMenuUI optionsMenuUI;


    //// Buttons ////
    // Main Container Buttons
    private Button PlayButton;
    private Button OptionsButton;
    private Button HelpButton;
    private Button QuitButton;

    //Help Container Buttons
    private Button BackButton_help;

    //Quit Container Buttons
    private Button QuitButton_ToDesktop;
    private Button QuitButton_ToMainMenu;
    private Button BackButton_quit;


    // Containers
    private VisualElement parentContainer;
    private VisualElement mainContainer;
    private VisualElement optionsContainer;
    private VisualElement helpContainer;
    private VisualElement quitContainer;

    //Audio
    [SerializeField]
    private AudioMixer audioMixer;
    private MenuAudioController menuAudioController;
    private BackgroundMusicController backgroundMusicController;


    // STATS UI
    VisualElement seedGroup;
    private GameSetup gameSetup;


    // SAVE AND QUIT
    private ScoreController scoreController;
    private GameStats gameStats;


    private void Awake()
    {
        //UI Document
        main_document = this.GetComponent<UIDocument>();

        //Audio
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();
        backgroundMusicController = GameObject.Find("BackgroundAudio").GetComponent<BackgroundMusicController>();

        //Options menu class script
        optionsMenuUI = this.GetComponent<OptionsMenuUI>();

        //Input - Listens for the Esc Key to back out of menus
        inputControls = GameObject.Find("InputController").GetComponent<InputController>();

        // Containers
        parentContainer = main_document.rootVisualElement.Q("Container");
        mainContainer = main_document.rootVisualElement.Q("MainContainer");
        quitContainer = main_document.rootVisualElement.Q("QuitScreen");
        optionsContainer = main_document.rootVisualElement.Q("OptionsContainer");
        helpContainer = main_document.rootVisualElement.Q("PlayingTheGame");

        //Seed-DungeonLevel UI Elements
        seedGroup = main_document.rootVisualElement.Q("SeedGroup");


        //// Get Buttons
        // Main Container Buttons
        PlayButton = main_document.rootVisualElement.Q("PlayButton") as Button;
        OptionsButton = main_document.rootVisualElement.Q("OptionsButton") as Button;
        HelpButton = main_document.rootVisualElement.Q("HelpButton") as Button;
        QuitButton = main_document.rootVisualElement.Q("QuitButton") as Button;

        //Help Container Buttons
        BackButton_help = main_document.rootVisualElement.Q("BackButtonHelp") as Button;

        //Quit Container Buttons
        QuitButton_ToDesktop = quitContainer.Q("ButtonExitToDesktop") as Button; //Quit Screen - Quit to Desktop
        QuitButton_ToMainMenu = quitContainer.Q("ButtonExitToMenu") as Button; //Quit Screen - Quit to Main Menu
        BackButton_quit = quitContainer.Q("BackButtonQuit") as Button; // Quit Screen - return to Pause menu


        //Assign buttons to their actions
        PlayButton.clicked += PlayGame; //Pause -> Gameplay
        OptionsButton.clicked += optionsMenuUI.GoToOptions; //Pause -> Options
        HelpButton.clicked += Open_HelpMenu; //Pause -> Help
        QuitButton.clicked += Open_QuitMenu; //Pause -> Quit
        BackButton_help.clicked += Close_HelpMenu; //Help -> Pause
        QuitButton_ToDesktop.clicked += SaveAndQuitToDesktop; //Quit -> Desktop
        QuitButton_ToMainMenu.clicked += SaveAndQuitToMain; //Quit -> Main Menu (scene change)
        BackButton_quit.clicked += Close_QuitMenu; //Quit -> Pause
        // Options -> Pause is controlled by a separate script


        // Initialize Containers
        // Make sure no containers are showing to start, even if Artisan left one enabled in the UI editor
        mainContainer.style.display = DisplayStyle.Flex;
        optionsContainer.style.display = DisplayStyle.None;
        helpContainer.style.display = DisplayStyle.None;
        quitContainer.style.display = DisplayStyle.None;
        parentContainer.style.display = DisplayStyle.None;

    }

    private void Start()
    {

        //Dungeon Seed and Level display
        Label seedValue = seedGroup.Q("SeedValue") as Label;
        gameSetup = GameObject.Find("GameSetup").GetComponent<GameSetup>();
        seedValue.text = "Dungeon Seed: " + gameSetup.seed.ToString();

        // SAVE AND QUIT
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();

    }

    private void OnEnable()
    {
        inputControls.OnMenuEnter += Event_OnMenuEnter;

    }
    private void OnDisable()
    {
        inputControls.OnMenuEnter -= Event_OnMenuEnter;

    }

    //Pause Menu parent container toggle
    private void TogglePauseMenu()
    {
        parentContainer.style.display = (parentContainer.style.display == DisplayStyle.Flex) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    //unpause the game
    private void PlayGame()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        TogglePauseMenu();
        inputControls.currentInputState = InputController.InputState.Gameplay;
        inputControls.ToggleMovement();
    }

    private void SaveGameBeforeQuit()
    {
        scoreController.ScoreRound();
        gameStats.IncrementCartographersLost();
        gameStats.SaveStats();
    }

    private void SaveAndQuitToMain()
    {

        //BUTTON SOUND
        menuAudioController.PlayAudioClip("ButtonClose");

        //save
        SaveGameBeforeQuit();

        //Load scene to main menu
        backgroundMusicController.ChangeSongForScene("Main Menu");
        gameStats.NewGame();
        SceneManager.LoadScene("Main Menu");
    }

    private void SaveAndQuitToDesktop()
    {
        //SAVE
        SaveGameBeforeQuit();

        //BUTTON SOUND
        menuAudioController.PlayAudioClip("ButtonClose");

        //QUIT
        Debug.Log("Quit Game");
        Application.Quit();
    }


    private void Open_HelpMenu()
    {
        //BUTTON SOUND
        menuAudioController.PlayAudioClip("ButtonClose");

        //manually set the state in InputController
        inputControls.currentInputState = InputController.InputState.PauseMenu_Sub;

        //Toggle containers
        mainContainer.style.display = DisplayStyle.None;
        helpContainer.style.display = DisplayStyle.Flex;
    }

    private void Close_HelpMenu()
    {
        //BUTTON SOUND
        menuAudioController.PlayAudioClip("ButtonClose");

        //manually set the state in InputController
        inputControls.currentInputState = InputController.InputState.PauseMenu;

        //Toggle containers
        mainContainer.style.display = DisplayStyle.Flex;
        helpContainer.style.display = DisplayStyle.None;
    }

    private void Open_QuitMenu()
    {
        //BUTTON SOUND
        menuAudioController.PlayAudioClip("ButtonClose");

        //manually set the state in InputController
        inputControls.currentInputState = InputController.InputState.PauseMenu_Sub;

        //Toggle containers
        mainContainer.style.display = DisplayStyle.None;
        quitContainer.style.display = DisplayStyle.Flex;
    }
    private void Close_QuitMenu()
    {
        //BUTTON SOUND
        menuAudioController.PlayAudioClip("ButtonClose");

        //manually set the state in InputController
        inputControls.currentInputState = InputController.InputState.PauseMenu;

        //Toggle containers
        mainContainer.style.display = DisplayStyle.Flex;
        quitContainer.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Event - thrown by InputController (when Esc key is pressed)
    /// </summary>
    public void Event_OnMenuEnter(object sender, InputController.MenuArgs e)
    {

        // Gameplay -> Pause
        if (e.inputState == InputController.InputState.Gameplay)
        {
            TogglePauseMenu();
            //movement is disabled and sound is played in InputController
        }

        // Options -> Pause
        else if (e.inputState == InputController.InputState.PauseMenu_Sub && optionsContainer.style.display == DisplayStyle.Flex)
        {
            optionsMenuUI.SaveSettings();
        }

        // Help -> Pause 
        // Quit -> Pause
        else if (e.inputState == InputController.InputState.PauseMenu_Sub)
        {
            helpContainer.style.display = DisplayStyle.None;
            quitContainer.style.display = DisplayStyle.None;
            mainContainer.style.display = DisplayStyle.Flex;

            inputControls.currentInputState = InputController.InputState.PauseMenu;
        }

        // Pause -> Gameplay
        else if (e.inputState == InputController.InputState.PauseMenu)
        {
            //Call the same function as the play button
            PlayGame();
        }
    }
}
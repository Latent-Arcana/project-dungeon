using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static DungeonNarrator;

public class InputController : MonoBehaviour
{

    private GameObject player;
    private bool movementEnabled = true;
    private Vector3 playerPosition;

    protected DungeonNarrator dungeonNarrator;


    ///// Audio ////
    private MenuAudioController menuAudioController;


    // Simple event handler for our input events
    public event EventHandler<InputArgs> OnInput;
    public event EventHandler OnMapEnter;
    public event EventHandler OnInventoryEnter;
    public event EventHandler<MenuArgs> OnMenuEnter;

    public InputState currentInputState = InputState.Loading;

    public GameObject loadingScreen;

    // Passing in the dirction to the event will allow us to send movement data to the player and enemies
    public class InputArgs : EventArgs
    {
        public Vector2 direction;
    }

    public class MenuArgs : EventArgs
    {
        public InputState inputState;
    }

    //TODO: implement this in the M and I key if statements, and move enum to the enums file
    public enum InputState
    {
        Loading,
        Gameplay,
        MapMenu,
        PauseMenu,
        InventoryMenu,
        PauseMenu_Sub,
    }

    private void OnEnable()
    {
        ObjectGeneration.AllRoomsPlacementComplete += CompletedObjectPlacement;
    }

    private void OnDisable()
    {

        ObjectGeneration.AllRoomsPlacementComplete -= CompletedObjectPlacement;

    }

    private void Awake()
    {
        menuAudioController = GameObject.Find("MenuAudio").GetComponent<MenuAudioController>();
    }


    void Start()
    {
        //get player
        player = GameObject.Find("Player");
        playerPosition = player.transform.position;

        loadingScreen = GameObject.Find("Loading");


        movementEnabled = true;

    }

    void CompletedObjectPlacement()
    {
        Debug.Log("completed all rooms");
        currentInputState = InputState.Gameplay;

        loadingScreen.SetActive(false);

        Dungeon_Narrator.InitiateScreenFadeIn();


        movementEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        //listen for all MOVEMENT inputs

        if (movementEnabled)
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            {
                // We send movement data to all entities that are listening for OnInput
                OnInput.Invoke(this, new InputArgs { direction = Vector2.up });
            }

            else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                // We send movement data to all entities that are listening for OnInput
                OnInput.Invoke(this, new InputArgs { direction = Vector2.down });
            }

            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                // We send movement data to all entities that are listening for OnInput
                OnInput.Invoke(this, new InputArgs { direction = Vector2.left });

            }

            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                // We send movement data to all entities that are listening for OnInput
                OnInput.Invoke(this, new InputArgs { direction = Vector2.right });
            }

        }


        //Menu and UI controls

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //Enter Pause Menu
            if (currentInputState == InputState.Gameplay)
            {
                PauseGame();
            }

            //Exit Pause Menu
            else if (currentInputState == InputState.PauseMenu)
            {
                ClosePauseMenu();
            }

            //Exit Inventory Screen
            else if (currentInputState == InputState.InventoryMenu)
            {
                CloseInventory();
            }

            //Exit Map Screen
            else if (currentInputState == InputState.MapMenu)
            {
                CloseMap();
            }

            //Exit a sub menu of the Pause Menu, go back to the Pause menu
            else if (currentInputState == InputState.PauseMenu_Sub)
            {
                menuAudioController.PlayAudioClip("ButtonClose");
                OnMenuEnter.Invoke(this, new MenuArgs { inputState = InputState.PauseMenu_Sub }); //throw event to InventoryUI to toggle UI element
            }

        }

        else if (Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.Tab))
        {
            //return to gameplay from Inv
            if (currentInputState == InputState.InventoryMenu)
            {
                CloseInventory();
            }

            //open inv
            else if (currentInputState == InputState.MapMenu)
            {
                //close map but dont return to gameplay
                //OnMapEnter.Invoke(this, EventArgs.Empty); //throw event to MapMenuUI to toggle UI element

                CloseMap();
                //open Inv
                OpenInventory();
            }

            else if (currentInputState == InputState.Gameplay)
            {
                OpenInventory();
            }
        }

        else if (Input.GetKeyUp(KeyCode.M) || Input.GetKeyUp(KeyCode.X))
        {
            //return to gameplay from Map
            if (currentInputState == InputState.MapMenu)
            {
                CloseMap();
            }

            //open inv
            else if (currentInputState == InputState.InventoryMenu)
            {
                //close inv but dont return to gameplay
                OnInventoryEnter.Invoke(this, EventArgs.Empty); //throw event to InventoryUI to toggle UI element

                //open Map
                OpenMap();
            }

            else if (currentInputState == InputState.Gameplay)
            {
                OpenMap();
            }
        }

    }


    //Functions to toggle game state, UI elements, and movement
    
    public void ReturnToGameplay()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        currentInputState = InputState.Gameplay;
        movementEnabled = true;
    }

    public void PauseGame()
    {
        menuAudioController.PlayAudioClip("ButtonOpen");
        OnMenuEnter.Invoke(this, new MenuArgs { inputState = InputState.Gameplay }); //throw event to InventoryUI to toggle UI element
        currentInputState = InputState.PauseMenu;
        movementEnabled = false;
    }

    public void ClosePauseMenu()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        OnMenuEnter.Invoke(this, new MenuArgs { inputState = InputState.PauseMenu }); //throw event to InventoryUI to toggle UI element
        ReturnToGameplay();
    }

    public void OpenMap()
    {
        menuAudioController.PlayAudioClip("MapOpen");
        Dungeon_Narrator.DisableDungeonNarrator();
        OnMapEnter.Invoke(this, EventArgs.Empty); //throw event to MapMenuUI to toggle UI element
        currentInputState = InputState.MapMenu;
        movementEnabled = false;
    }

    public void CloseMap()
    {
        menuAudioController.PlayAudioClip("MapClose");
        Dungeon_Narrator.EnableDungeonNarrator();
        OnMapEnter.Invoke(this, EventArgs.Empty); //throw event to MapMenuUI to toggle UI element
        //ReturnToGameplay();
        currentInputState = InputState.Gameplay;
        movementEnabled = true;
    }

    public void OpenInventory()
    {
        menuAudioController.PlayAudioClip("ButtonOpen");
        OnInventoryEnter.Invoke(this, EventArgs.Empty); //throw event to InventoryUI to toggle UI element
        currentInputState = InputState.InventoryMenu;
        movementEnabled = false;

    }

    public void CloseInventory()
    {
        menuAudioController.PlayAudioClip("ButtonClose");
        OnInventoryEnter.Invoke(this, EventArgs.Empty); //throw event to InventoryUI to toggle UI element
        ReturnToGameplay();
    }

    /// <summary>
    /// used to toggle the movementEnabled variable, which controlls if the WASD keys are able to move the player
    /// </summary>
    public void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
    }

    public void PlayerInputDeath()
    {
        movementEnabled = false;
    }

}

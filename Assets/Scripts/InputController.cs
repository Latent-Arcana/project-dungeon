using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputController : MonoBehaviour
{

    private GameObject player;

    private bool movementEnabled = true;
    private bool pauseMenuOpen = false;
    private bool mapOpen = false;

    private Vector3 playerPosition;

    // Simple event handler for our input events
    public event EventHandler<InputArgs> OnInput;
    public event EventHandler OnMapEnter;

    // Passing in the dirction to the event will allow us to send movement data to the player and enemies
    public class InputArgs : EventArgs
    {
        public Vector2 direction;
    }

    
    void Start()
    {
        //get player
        player = GameObject.Find("Player");
        playerPosition = player.transform.position;

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

        //Map only usable when not in the menu
        if (Input.GetKeyUp(KeyCode.M) && !pauseMenuOpen)
        {
            ToggleMap();

        }

        //Note: Esc Key input is still located in the MainMenuController file
    }

    public void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
    }

    //Called by the MainMenuUI script when the menu is opened
    public void TogglePauseMenu()
    {
        pauseMenuOpen = !pauseMenuOpen;

        //If the menu was just opened and the map was already open, close the map
        if (pauseMenuOpen && mapOpen)
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        mapOpen = !mapOpen;
        //note: this is used for entering and exiting the map
        //calls the map toggle in MapMenuUI
        OnMapEnter.Invoke(this, EventArgs.Empty);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Attached to the gameobject that holds the collider(s) for room and hallway
public class RoomFogController : MonoBehaviour
{


    // public int roomId = -1;

    // public GameObject fogSpriteObject;
    // public PlayerMovement playerMovement;

    // void Start()
    // {
    //     roomId = this.gameObject.GetComponentInParent<Room>().roomId;
    //     fogSpriteObject = this.gameObject.transform.GetChild(0).gameObject;
    //     playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();



    //     // if (roomId == 0)
    //     // {
    //     //     fogSpriteObject.SetActive(false);
    //     // }


    // }

    // private void OnEnable()
    // {

    //     if (playerMovement == null)
    //     {

    //         playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

    //     }

    //     playerMovement.OnRoomEnter += RoomFog_OnRoomEnter;

    // }

    // private void OnDisable()
    // {
    //     playerMovement.OnRoomEnter -= RoomFog_OnRoomEnter;
    // }

    // private void RoomFog_OnRoomEnter(object sender, PlayerMovement.InputArgs e)
    // {
    //     if (fogSpriteObject == null)
    //     {
    //         fogSpriteObject = this.gameObject.transform.GetChild(0).gameObject;
    //     }

    //     if (e.type == "enter" && e.roomId == roomId)
    //     {
    //         //collided with me
    //         //fogSpriteObject.SetActive(false);
    //     }

    //     else
    //     {
    //         //room or hallway change, I should not be visible now
    //         //fogSpriteObject.SetActive(true);
    //     }

    // }

}

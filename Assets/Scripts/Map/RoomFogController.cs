using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerMovement;

//Attached to the gameobject that holds the collider(s) for room and hallway
public class RoomFogController : MonoBehaviour
{


    public int roomId = -1;

    public GameObject fogSpriteObject;

    void Start()
    {
        roomId = this.gameObject.GetComponentInParent<Room>().roomId;
        fogSpriteObject = this.gameObject.transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        Player_Movement.OnRoomEnter += RoomFog_OnRoomEnter;

    }

    private void OnDisable()
    {
        Player_Movement.OnRoomEnter -= RoomFog_OnRoomEnter;
    }

    private void RoomFog_OnRoomEnter(object sender, PlayerMovement.InputArgs e)
    {


        if (e.type == "enter" && e.roomId == roomId)
        {
            //collided with me
            fogSpriteObject.SetActive(false);
        }

        else
        {
            //room or hallway change, I should not be visible now
            fogSpriteObject.SetActive(true);
        }

    }

}

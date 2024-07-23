using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Attached to the gameobject that holds the collider(s) for room and hallway
public class HallwayFogController : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision){


    }
    // private PlayerMovement playerMovement;


    // //for hallways
    // private int room1Id = -1;
    // private int room2Id = -1;

    // public SpriteRenderer fogSprite;

    // void Awake()
    // {
    //     playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    // }

    // void Start()
    // {
    //     room1Id = this.gameObject.GetComponentInParent<Hallway>().room1Id;
    //     room2Id = this.gameObject.GetComponentInParent<Hallway>().room2Id;

    //     fogSprite = this.gameObject.GetComponent<SpriteRenderer>();
    // }

    // private void OnEnable()
    // {
    //     playerMovement.OnRoomEnter += HallwayFog_OnRoomEnter;
    // }

    // private void OnDisable()
    // {
    //     playerMovement.OnRoomEnter -= HallwayFog_OnRoomEnter;
    // }

    // private void HallwayFog_OnRoomEnter(object sender, PlayerMovement.InputArgs e)
    // {

    //     if (e.type == "hallway" && e.room1Id == room1Id && e.room2Id == room2Id)
    //     {
    //         //collided with me and I'm a hallway
    //         fogSprite.enabled = false;
    //     }

    //     else
    //     {
    //         //room or hallway change, I should not be visible now
    //         fogSprite.enabled = true;
    //     }

    // }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static PlayerMovement;

public class MapController : MonoBehaviour
{
    // POTENTIAL OPTIMIZATION:
    // Initialize the map here after BSP is done, after the game has "started"


    [SerializeField]
    GameObject fogBoxPrefab;

    GameObject[] fogBoxes;


    private List<GameObject> allRooms;



    void Awake()
    {
        allRooms = GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allRooms;

        //This way we can hide the room's partition/fogbox by roomId, instead of searching for the game object by name
        fogBoxes = new GameObject[allRooms.Count];

        FogOfWar();
    }


    private void OnEnable()
    {
        //subscribe MapController to the PlayerMovement script's OnRoomEnter event

        Player_Movement.OnRoomEnter += Event_OnRoomEnter;

    }

    private void OnDisable()
    {
        Player_Movement.OnRoomEnter -= Event_OnRoomEnter;
    }

    void FogOfWar()
    {

        //For organization in the scene heirarchy
        GameObject partitionParent = GameObject.Find("Partitions");

        foreach( GameObject roomParent in allRooms )
        {

            Room room = roomParent.GetComponent<Room>();

            //NOTE: The math here requires the fog sprite to have its pivot point set to lower left (in sprite editor) and the sprite renderer to 


            //testing different offsets
            //GameObject fogBox = Instantiate(fogBoxPrefab, new Vector3(room.partition.x - 500.5f, room.partition.y-500.5f, 0f), Quaternion.identity); //half unit is to account for wall edges that are past the partition, not the movement grid
            //fogBox.transform.localScale = new Vector3(room.partition.width+1, room.partition.height+1, 0); //+1 here is to account for the half unit above
            GameObject fogBox = Instantiate(fogBoxPrefab, new Vector3(room.partition.x - 500, room.partition.y-500, 0), Quaternion.identity); 
            fogBox.transform.localScale = new Vector3(room.partition.width, room.partition.height, 0); 
            fogBox.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");


            //For organization in the scene heirarchy
            fogBox.transform.SetParent(partitionParent.transform);
            fogBox.name = "Partition" + room.roomId;

            fogBoxes[room.roomId] = fogBox;

        }

        //TODO: Player might not always spawn in first room
        // change to figure out where player spawned
        // But - be aware of timing, as this script might run after the player spawns in

        RemoveFog(0);

    }

    void RemoveFog(int roomId){
        if (fogBoxes[roomId] != null)
        {
            fogBoxes[roomId].SetActive(false);
        }
    }


    private void Event_OnRoomEnter(object sender, PlayerMovement.InputArgs e){
        //Debug.Log("Entered room" + e.roomId);
        RemoveFog(e.roomId);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class MapController : MonoBehaviour
{

    [SerializeField]
    GameObject fogBoxPrefab;

    GameObject[] fogBoxes;

    private List<GameObject> allRooms;

    PlayerMovement playerMovement;


    void Awake()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        //subscribe MapController to the PlayerMovement script's OnRoomEnter event
        playerMovement.OnRoomEnter += Event_OnRoomEnter;

    }

    private void OnDisable()
    {
        playerMovement.OnRoomEnter -= Event_OnRoomEnter;
    }

    public void FogOfWar()
    {

        allRooms = GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allRooms;
        fogBoxes = new GameObject[allRooms.Count];

        //For organization in the scene heirarchy
        GameObject partitionParent = GameObject.Find("Partitions");

        foreach (GameObject roomParent in allRooms)
        {

            Room room = roomParent.GetComponent<Room>();

            //NOTE: The math here requires the fog sprite to have its pivot point set to lower left (in sprite editor) and the sprite renderer to 


            //testing different offsets
            //GameObject fogBox = Instantiate(fogBoxPrefab, new Vector3(room.partition.x - 500.5f, room.partition.y-500.5f, 0f), Quaternion.identity); //half unit is to account for wall edges that are past the partition, not the movement grid
            //fogBox.transform.localScale = new Vector3(room.partition.width+1, room.partition.height+1, 0); //+1 here is to account for the half unit above
            GameObject fogBox = Instantiate(fogBoxPrefab, new Vector3(room.partition.x - 500, room.partition.y - 500, 0), Quaternion.identity);
            fogBox.transform.localScale = new Vector3(room.partition.width, room.partition.height, 0);
            fogBox.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");


            //For organization in the scene heirarchy
            fogBox.transform.SetParent(partitionParent.transform);
            fogBox.name = "Partition" + room.roomId;

            fogBoxes[room.roomId] = fogBox;

            //RemoveFog(room.roomId);

        }

        //TODO: Player might not always spawn in first room
        // change to figure out where player spawned
        // But - be aware of timing, as this script might run after the player spawns in


        RemoveFog(0);

    }

    public void RemoveFog(int roomId)
    {
        if (fogBoxes[roomId] != null)
        {
            fogBoxes[roomId].SetActive(false);
        }
    }


    private void Event_OnRoomEnter(object sender, PlayerMovement.InputArgs e)
    {
        //Debug.Log("Entered room" + e.roomId);
        RemoveFog(e.roomId);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfBehavior : MonoBehaviour
{
    // Dungeon Data
    GameSetup gameSetup;

    MapMarker mapMarker;

    MapController mapController;

    public Room bookRoom;
    public bool hasBeenUsed = false;
    void Awake()
    {
        gameSetup = GameObject.Find("GameSetup").GetComponent<GameSetup>();
        mapMarker = GameObject.Find("MapController").GetComponent<MapMarker>();
        mapController = GameObject.Find("MapController").GetComponent<MapController>();
    }

    void Start()
    {
        if (gameSetup.roomsGenerated)
        {
            bookRoom = gameSetup.GetRandomRoom();
        }
    }


    public void RevealRoom()
    {
        // bookshelves have a 30% chance of dropping a book
        float randDropChance = UnityEngine.Random.value;

        if (randDropChance > .3f)
        {
            DungeonNarrator.Dungeon_Narrator.AddBookshelfText(bookRoom.roomType, false);
            hasBeenUsed = true;
            return;
        }
        DungeonNarrator.Dungeon_Narrator.AddBookshelfText(bookRoom.roomType, true);
        mapMarker.PlacePresetMarker(bookRoom);
        mapController.RemoveFog(bookRoom.roomId);
        hasBeenUsed = true;

    }


}

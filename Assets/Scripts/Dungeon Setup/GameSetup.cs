using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemLoader;
using static ContainerGeneration;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System;

public class GameSetup : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField]
    [Tooltip("Set DEBUG_SeedGen to true if you want to use the same seed over and over.")]
    public bool DEBUG_SeedGen;

    [SerializeField]
    [Tooltip("If you want to use the same seed over and over (DEBUG_SeedGen), remember to set the seed value here")]
    public int seed = 0;

    [Header("GAMEPLAY")]
    public BSPGeneration bspController;
    public EnemyGeneration dangerGenerator;
    public ObjectGeneration objectGenerator;
    public MapController mapController;
    public MapMarker mapMarker;
    public FloorCoveringGeneration floorCoverGenerator;

    public int[] portalSeeds;

    public List<int> seedsInDungeon;

    public Camera mapCamera;

    public ExplorationData loadedExplorationData;

    public List<int> seedsInSaveData;

    public bool roomsGenerated;


    void Awake()
    {
        mapCamera = GameObject.Find("Map Camera").GetComponent<Camera>();
        mapController = GameObject.Find("MapController").GetComponent<MapController>();
        mapMarker = GameObject.Find("MapController").GetComponent<MapMarker>();

        GameObject Dungeon_Generator = GameObject.Find("DungeonGenerator");
        dangerGenerator = Dungeon_Generator.GetComponent<EnemyGeneration>();
        objectGenerator = Dungeon_Generator.GetComponent<ObjectGeneration>();
        bspController = Dungeon_Generator.GetComponent<BSPGeneration>();
        floorCoverGenerator = Dungeon_Generator.GetComponent<FloorCoveringGeneration>();


        // EVEN NOW THE EVIL SEED OF WHAT YOU'VE DONE, GERMINATES WITHIN YOU!!!!1
        // (Seed generation based on what you enter in the editor)

        loadedExplorationData = SaveSystem.LoadPlayerSaveData();
        if (loadedExplorationData != null)
        {
            seedsInSaveData = loadedExplorationData.mappedDungeons;

        }

        if (!DEBUG_SeedGen)
        {
            seed = CreateSeed();
        }

        else // if we want to regenerate the same seed over and over ( debug seedgen is true)
        {
            UnityEngine.Random.InitState(seed);
        }

        seedsInDungeon = new() { seed };



    }

    //TODO: potential optimization: turn some of these into coroutines in their respective scripts, 
    // so you aren't running them in series
    void Start()
    {
        // kick off the first step here and run in order

        // Loading Items from item data, singleton syntax is ItemLoader.Item_Loader

        if (!Item_Loader.hasLoadedItemsSuccessfully && !Item_Loader.hasLoadedLootTablesSuccessfully)
        {
            Item_Loader.LoadItemsFromJson();
            Item_Loader.LoadLootTablesFromJson();
            //Debug.Log("First time loading Item Data");
        }

        List<Item> itemsDatabase = Item_Loader.GetItemsDatabase();

        LootTable lootTable = Item_Loader.GetLootTable();

        // Generating container data so we can send information to each container in the game
        // At this point, ContainerBehavior.cs on each individual openable object is what handles grabbing container data for itself
        // The generator produces this via functions within it as a singleton
        Container_Generator.InitializeContainerGenerator(itemsDatabase, lootTable);

        GameStats gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();

        Debug.Log("Current Dungeon Level is: " + gameStats.currentDungeonLevel);
        //bsp
        bspController.StartBspGeneration(gameStats.currentDungeonLevel);

        // adjust the camera position and scale based on the ratio of the map
        if (gameStats.currentDungeonLevel >= 5)
        {
            // Map grows by 1 in both width and height per level
            int mapSize = 50 + gameStats.currentDungeonLevel; // Map size starts at 50x50 and maxes out at 100x100

            // Update orthographic size (this part is correct)
            float initialOrthographicSize = 40f; // Starting orthographic size
            float initialMapSize = 50f; // Starting map size
            mapCamera.orthographicSize = initialOrthographicSize + ((mapSize - initialMapSize) / 2f);

            // Calculate the center of the map based on the map size
            float mapCenterX = mapSize / 2f;
            float mapCenterY = mapSize / 2f;

            // Adjust the camera's position to center on the growing map
            // Initial camera position is -480, -500 for the initial map size (50x50)
            float initialCameraPosX = -480f;
            float initialCameraPosY = -500f;

            // Calculate new position based on growth from initial center
            float newCameraPosX = initialCameraPosX + mapCenterX;
            float newCameraPosY = initialCameraPosY + mapCenterY;

            // Update camera position
            mapCamera.transform.position = new Vector3(newCameraPosX + 5f, newCameraPosY, mapCamera.transform.position.z);


        }

        roomsGenerated = true;

        portalSeeds = new int[bspController.allRooms.Count];

        // for every room, except the starting room, give it a pregenerated portal seed
        for (int i = 1; i < portalSeeds.Length; ++i)
        {

            int randSeed = CreateSeed();

            portalSeeds[i] = UnityEngine.Random.Range(0, 10000);

            seedsInDungeon.Add(randSeed);
        }

        //enemy


        //objects
        objectGenerator.GenerateObjectPlacements(bspController.allRooms);

        //floor covering
        floorCoverGenerator.GenerateGroundCover(bspController.allRooms);


        //map
        mapController.FogOfWar();

        mapMarker.PlacePresetMarker(bspController.allRooms[0].GetComponent<Room>());
    }

    /// <summary>
    /// Recursive function call to receive a random number that is valid for generating a seed. 
    /// The seeds need to be unique within the dungeon.
    /// </summary>

    int CreateSeed()
    {
        int randSeed = UnityEngine.Random.Range(0, 10000);

        if (randSeed == seed)
        {
            CreateSeed();
        }

        else if (seedsInDungeon != null && seedsInDungeon.Contains(randSeed))
        {
            CreateSeed();
        }

        else if (seedsInSaveData != null && seedsInSaveData.Contains(randSeed))
        {
            CreateSeed();
        }

        return randSeed;
    }

    public Room GetRandomRoom()
    {
        int randRoomIndex = UnityEngine.Random.Range(1, bspController.allRooms.Count);

        return bspController.allRooms[randRoomIndex].GetComponent<Room>();
    }



    //TODO: maybe catch events here, if we don't want to wait for the whole funtion to return



    //TODO: Throw events over to the Loading screen UI

}

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
    [SerializeField]
    [Tooltip("Keep in mind, dungeon level is 1-indexed")]
    public int DEBUG_dungeonLevel = 1;

    [Header("GAMEPLAY")]
    public BSPGeneration bspController;
    public EnemyGeneration dangerGenerator;
    public ObjectGeneration objectGenerator;
    public MapController mapController;
    public MapMarker mapMarker;
    public FloorCoveringGeneration floorCoverGenerator;

    public Camera mapCamera;

    public ExplorationData loadedExplorationData;

    public List<int> seedsInSaveData;

    public bool roomsGenerated;

    public GameStats gameStats;


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
            UnityEngine.Random.InitState(seed);
        }

        else // if we want to regenerate the same seed over and over ( debug seedgen is true)
        {
            UnityEngine.Random.InitState(seed);
        }


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
        }

        List<Item> itemsDatabase = Item_Loader.GetItemsDatabase();

        LootTable lootTable = Item_Loader.GetLootTable();

        // Generating container data so we can send information to each container in the game
        // At this point, ContainerBehavior.cs on each individual openable object is what handles grabbing container data for itself
        // The generator produces this via functions within it as a singleton
        Container_Generator.InitializeContainerGenerator(itemsDatabase, lootTable);

        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();
        
        if (DEBUG_SeedGen)
        {
            gameStats.currentDungeonLevel = DEBUG_dungeonLevel;
        }
        else{
            DEBUG_dungeonLevel = gameStats.currentDungeonLevel;
        }


        bspController.StartBspGeneration(gameStats.currentDungeonLevel);


        // adjust the camera position and scale based on the ratio of the map

        // Map grows by 1 in both width and height per level
        int mapSize = 25 + gameStats.currentDungeonLevel;

        // Update orthographic size (this part is correct)
        float initialOrthographicSize = 25f; // Starting orthographic size
        float initialMapSize = 25f; // Starting map size
        mapCamera.orthographicSize = Math.Min(initialOrthographicSize + ((mapSize - initialMapSize) / 2f), initialOrthographicSize + 30);

        // Calculate the center of the map based on the map size
        float mapCenterX = Math.Min(mapSize / 2, 42);
        float mapCenterY = Math.Min(mapSize / 2, 42);

        // Adjust the camera's position to center on the growing map
        // Initial camera position is -480, -500 for the initial map size (50x50)
        float initialCameraPosX = -500f;
        float initialCameraPosY = -500f;

        // Calculate new position based on growth from initial center
        float newCameraPosX = (initialCameraPosX + mapCenterX) + (.7f * mapCenterX); //mapCenterX;
        float newCameraPosY = initialCameraPosY + mapCenterY;


        // Update camera position
        mapCamera.transform.localPosition = new Vector3(newCameraPosX, newCameraPosY, 0);


        roomsGenerated = true;
        //enemy


        //objects
        objectGenerator.GenerateObjectPlacements(bspController.allRooms);

        //floor covering
        floorCoverGenerator.GenerateGroundCover(bspController.allRooms);


        //map
        mapController.FogOfWar();

    }

    /// <summary>
    /// Recursive function call to receive a random number that is valid for generating a seed. 
    /// The seeds need to be unique within the dungeon.
    /// </summary>

    int CreateSeed()
    {
        int randSeed = UnityEngine.Random.Range(0, int.MaxValue);

        if (randSeed == seed)
        {
            CreateSeed();
        }

        // else if (seedsInDungeon != null && seedsInDungeon.Contains(randSeed))
        // {
        //     CreateSeed();
        // }

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

    public string GetFormattedSeedData()
    {
        return seed + "-" + gameStats.currentDungeonLevel;
    }

}

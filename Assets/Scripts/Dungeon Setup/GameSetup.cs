using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemLoader;
using static ContainerGeneration;
using Unity.VisualScripting;

public class GameSetup : MonoBehaviour
{
    [SerializeField]
    public int seed = 0;
    public BSPGeneration bspController;
    public EnemyGeneration dangerGenerator;
    public ObjectGeneration objectGenerator;
    public MapController mapController;
    public FloorCoveringGeneration floorCoverGenerator;

    public int[] portalSeeds;

    public List<int> seedsInDungeon;

    

    void Awake()
    {
        mapController = GameObject.Find("MapController").GetComponent<MapController>();

        GameObject Dungeon_Generator = GameObject.Find("DungeonGenerator");
        dangerGenerator = Dungeon_Generator.GetComponent<EnemyGeneration>();
        objectGenerator = Dungeon_Generator.GetComponent<ObjectGeneration>();
        bspController = Dungeon_Generator.GetComponent<BSPGeneration>();
        floorCoverGenerator = Dungeon_Generator.GetComponent<FloorCoveringGeneration>();

        // EVEN NOW THE EVIL SEED OF WHAT YOU'VE DONE, GERMINATES WITHIN YOU!!!!1
        // (Seed generation based on what you enter in the editor)

        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(1, int.MaxValue);

            UnityEngine.Random.InitState(seed);
        }
        else
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


        //bsp
        bspController.StartBspGeneration();

        portalSeeds = new int[bspController.allRooms.Count];

        // for every room, except the starting room, give it a pregenerated portal seed
        for (int i = 1; i < portalSeeds.Length; ++i)
        {

            int randSeed = CreateSeed();

            portalSeeds[i] = UnityEngine.Random.Range(1, int.MaxValue);

            seedsInDungeon.Add(randSeed);
        }

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
        int randSeed = UnityEngine.Random.Range(1, int.MaxValue);

        if (randSeed == seed)
        {
            CreateSeed();
        }

        else if (seedsInDungeon.Contains(randSeed))
        {
            CreateSeed();
        }

        return randSeed;
    }



    //TODO: maybe catch events here, if we don't want to wait for the whole funtion to return



    //TODO: Throw events over to the Loading screen UI

}

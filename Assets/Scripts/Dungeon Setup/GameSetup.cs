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


    void Awake()
    {
        mapController = GameObject.Find("MapController").GetComponent<MapController>();

        GameObject Dungeon_Generator = GameObject.Find("DungeonGenerator");
        dangerGenerator = Dungeon_Generator.GetComponent<EnemyGeneration>();
        objectGenerator = Dungeon_Generator.GetComponent<ObjectGeneration>();
        bspController = Dungeon_Generator.GetComponent<BSPGeneration>();

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

    }

    //TODO: potential optimization: turn some of these into coroutines in their respective scripts, 
    // so you aren't running them in series
    void Start()
    {
        // kick off the first step here and run in order

        // Loading Items from item data, singleton syntax is ItemLoader.Item_Loader

        if (!Item_Loader.hasLoadedSuccessfully)
        {
            Item_Loader.LoadItemsFromJson();
            Debug.Log("First time loading Item Data");
        }

        List<Item> itemsDatabase = Item_Loader.GetItemsDatabase();

        // Generating container data so we can send information to each container in the game
        // At this point, ContainerBehavior.cs on each individual openable object is what handles grabbing container data for itself
        // The generator produces this via functions within it as a singleton
        Container_Generator.InitializeContainerGenerator(itemsDatabase);


        //bsp
        bspController.StartBspGeneration();

        //enemy


        //objects
        objectGenerator.GenerateObjectPlacements(bspController.allRooms);

        //map
        mapController.FogOfWar();
    }



    //TODO: maybe catch events here, if we don't want to wait for the whole funtion to return



    //TODO: Throw events over to the Loading screen UI

}

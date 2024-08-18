using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

    public BSPGeneration bspController;
    public EnemyGeneration dangerGenerator;
    public ObjectGeneration objectGenerator;
    public MapController mapController;

    public ItemLoader itemLoader;


    void Awake()
    {
        mapController = GameObject.Find("MapController").GetComponent<MapController>();

        GameObject Dungeon_Generator = GameObject.Find("DungeonGenerator");
        dangerGenerator = Dungeon_Generator.GetComponent<EnemyGeneration>();
        objectGenerator = Dungeon_Generator.GetComponent<ObjectGeneration>();
        bspController = Dungeon_Generator.GetComponent<BSPGeneration>();
        itemLoader = Dungeon_Generator.GetComponent<ItemLoader>();

    }

    //TODO: potential optimization: turn some of these into coroutines in their respective scripts, 
    // so you aren't running them in series
    void Start()
    {
        // kick off the first step here and run in order
       
        // Loading Items from item data
        itemLoader.LoadItemsFromJson();
        List<Item> itemsDatabase = itemLoader.GetItemsDatabase();

        // TODO: Generating container data so we can send information to each container in the game

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

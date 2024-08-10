using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

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

    }

    void Start()
    {
        //kick off the first step here and run in order


        //TODO: potential optimization: turn some of these into coroutines in their respective scripts, 
        // so you aren't running them in series

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

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoreGeneration : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Object Prefabs")]

    [SerializeField]
    public GameObject[] loreObjects;

    public void Awake()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateLore(GameObject roomObj)
    {

        Room room = roomObj.GetComponent<Room>();


        // First let's figure out what kind of Lore room we are
        // The factory randomly chooses

        IRoomPopulator populator = RoomPopulatorFactory.GetPopulator(loreObjects);

        populator.PopulateRoom(tilemap, room);

        // if(subCategory == 2) // Treasure
        // {       
        //     int attempt = 0;
        //     bool objPlaced = false;

        //     while (attempt < 20 && !objPlaced)
        //     {

        //         Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width), UnityEngine.Random.Range(room.y + 1, room.y + room.height), 0);

        //         if(PlaceObject(position, room, loreObjects[0])){
                    
        //             Instantiate(loreObjects[0], position, Quaternion.identity);
                    
        //             placedObjects.Add(position, true);
                    
        //             objPlaced = true;
        //         }

                
        //         ++attempt;
        //     }

        // }

        // else if(subCategory == 3){
        //     int attempt = 0;
        //     bool objPlaced = false;

        //     while (attempt < 20 && !objPlaced)
        //     {

        //         Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

        //         if(PlaceObject(position, room, loreObjects[1])){
                    
        //             Instantiate(loreObjects[1], position, Quaternion.identity);
                    
        //             placedObjects.Add(position, true);
                    
        //             objPlaced = true;
        //         }

                
        //         ++attempt;
        //     }
        




       

    }

}



// USEFUL CODE FOR LATER
// if(objectBehavior == null){

//     return false;
// }

// IPlaceable objectPlacement = objectBehavior.loreObject as IPlaceable;

// if(objectPlacement == null){

//     return false;
// }
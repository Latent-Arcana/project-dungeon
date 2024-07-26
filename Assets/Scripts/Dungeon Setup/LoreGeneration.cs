using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoreGeneration : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Object Prefabs")]

    [SerializeField]
    public GameObject[] objectPrefabs;

    public void Start()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateLore(GameObject roomObj){

        Debug.Log("hello");
        Room room = roomObj.GetComponent<Room>();

        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width - 1), UnityEngine.Random.Range(room.y + 1, room.y + room.height - 1), 0);

        //UnityEngine.Random.Range(0, )

        /*
            // Pick a number of objects to try to fit into the room
            // Attempt to place those objects in the room 15 times
                // Randomly select a valid object from the list
                // Attempt to place it in a randomly selected location
                    If we fail, move on
                
            
            // If you fail all 15 times, try a fewer number of items

        */
        



    }

    private GameObject PlaceObject(GameObject prefab, Vector3 position){
        
        return Instantiate(objectPrefabs[0], (Vector3)position, Quaternion.identity);

    }

}

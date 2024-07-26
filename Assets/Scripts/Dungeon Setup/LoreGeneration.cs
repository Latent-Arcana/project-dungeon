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

        
        GameObject placedObject = Instantiate(objectPrefabs[0], (Vector3)position, Quaternion.identity);

    }

}

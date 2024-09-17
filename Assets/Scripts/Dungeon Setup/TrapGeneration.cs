using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TrapGeneration : MonoBehaviour
{
    [Header("Trap Prefabs")]

    [SerializeField]
    GameObject[] trapPrefabs;

    Tilemap tilemap;


    void Start()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateTrap(GameObject roomObject)
    {

        Room room = roomObject.GetComponent<Room>();

        int randTrap = UnityEngine.Random.Range(0, trapPrefabs.Length);

        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width - 1), UnityEngine.Random.Range(room.y + 1, room.y + room.height - 1), 0);


        PlaceTrap(trapPrefabs[randTrap], position, roomObject, 1);

    }

    // Place a trap
    public void PlaceTrap(GameObject trapPrefab, Vector3Int position, GameObject roomObject, int id)
    {

        GameObject gameplayRoom = roomObject.transform.GetChild(1).gameObject;
        Room room = roomObject.GetComponent<Room>();

        GameObject placedTrap = Instantiate(trapPrefab, (Vector3)position, Quaternion.identity);

        placedTrap.name = "Trap_" + room.roomId + "_ " + id;

        placedTrap.transform.SetParent(gameplayRoom.transform, true);
    }
}

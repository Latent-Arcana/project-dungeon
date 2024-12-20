using System.Collections;
using System.Collections.Generic;
using static BSPGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyGeneration : MonoBehaviour
{

    [Header("Enemy Prefabs")]

    [SerializeField]
    GameObject[] enemyPrefabs;

    Tilemap tilemap;

    public void Start()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }


    public void GenerateEnemies(GameObject roomObject)
    {

        Room room = roomObject.GetComponent<Room>();

        int randEnemyPreset = UnityEngine.Random.Range(0, enemyPrefabs.Length);

        int randEnemyCount = UnityEngine.Random.Range(1, 3);

        Dictionary<Vector3Int, bool> enemyMap = new Dictionary<Vector3Int, bool>();


       // Debug.Log("Placing at most " + randEnemyCount + " enemies in " + room.roomId);

        for(int i = 0; i < randEnemyCount; ++i)
        {

            Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width - 1), UnityEngine.Random.Range(room.y + 1, room.y + room.height - 1), 0);

            if (!enemyMap.ContainsKey(position))
            {
                enemyMap.Add(position, true);

                PlaceEnemy(enemyPrefabs[randEnemyPreset], position, roomObject, i);

            }
        }


    }

    // Place a specific enemy
    public void PlaceEnemy(GameObject enemyPrefab, Vector3Int position, GameObject roomObject, int id)
    {

        GameObject gameplayRoom = roomObject.transform.GetChild(1).gameObject;
        Room room = roomObject.GetComponent<Room>();

        GameObject placedEnemy = Instantiate(enemyPrefab, (Vector3)position, Quaternion.identity);

        placedEnemy.name = "Enemy_" + room.roomId + "_ " + id;

        placedEnemy.transform.SetParent(gameplayRoom.transform, true);

        EnemyBehavior enemyBehavior = placedEnemy.GetComponent<EnemyBehavior>();

        enemyBehavior.id = id;
        enemyBehavior.room = room.roomId;
        enemyBehavior.originPoint = (Vector2Int)position;
        enemyBehavior.AssignStats();
    }
}

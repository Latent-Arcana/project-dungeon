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

    GameStats gameStats;

    public void Start()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();

        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();
    }


    public void GenerateEnemies(GameObject roomObject)
    {

        int randEnemyPreset = UnityEngine.Random.Range(0, enemyPrefabs.Length);


        EnemyBehavior enemyBehavior = enemyPrefabs[randEnemyPreset].GetComponent<EnemyBehavior>(); // The behavior data of the enemy prefab

        int randEnemyCount = 1;

        switch (enemyBehavior.enemyType)
        {
            case Enums.EnemyType.Skeleton:

                randEnemyCount = UnityEngine.Random.Range(1, 4);
                break;

            case Enums.EnemyType.Kobold:
                randEnemyCount = UnityEngine.Random.Range(1, 5);
                break;

            case Enums.EnemyType.Goblin:
                randEnemyCount = UnityEngine.Random.Range(1, 3);
                break;

            case Enums.EnemyType.Bugbear:
                randEnemyCount = UnityEngine.Random.Range(1, 3);
                break;

            case Enums.EnemyType.Spirit:
                randEnemyCount = UnityEngine.Random.Range(1, 3);
                break;

        }


        PlaceEnemies(enemyPrefabs[randEnemyPreset], randEnemyCount, roomObject);


    }

    public void PlaceEnemies(GameObject enemyPrefab, int count, GameObject roomObject)
    {
        Debug.Log("placing enemies");
        Room room = roomObject.GetComponent<Room>();

        Dictionary<Vector3Int, bool> enemyMap = new Dictionary<Vector3Int, bool>();


        for (int i = 0; i < count; ++i)
        {

            Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width - 1), UnityEngine.Random.Range(room.y + 1, room.y + room.height - 1), 0);

            if (!enemyMap.ContainsKey(position))
            {
                enemyMap.Add(position, true);

                PlaceEnemy(enemyPrefab, position, roomObject, i);

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
        enemyBehavior.AssignStats(gameStats.currentDungeonLevel);
    }
}

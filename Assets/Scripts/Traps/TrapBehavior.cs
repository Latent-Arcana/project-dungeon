using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
public class TrapBehavior : MonoBehaviour
{

    public int roomId;

    public GameObject[] projectilePrefabs;

    private GameObject player;
    private PlayerMovement playerMovement;
    private bool playerInRoom;

    private List<GameObject> projectiles;


    public void Awake()
    {
        player = GameObject.Find("Player");

        playerMovement = player.GetComponent<PlayerMovement>();

        projectiles = new List<GameObject>();
    }

    public void OnEnable()
    {
        playerMovement.OnPlayerMoved += Input_OnPlayerMoved;
        playerMovement.OnRoomEnter += PlayerMovement_OnRoomEnter;
    }


    public void OnDisable()
    {
        playerMovement.OnPlayerMoved -= Input_OnPlayerMoved;
        playerMovement.OnRoomEnter -= PlayerMovement_OnRoomEnter;

    }

    private void Input_OnPlayerMoved(object sender, PlayerMovement.MovementArgs e)
    {
        float spawnChance = UnityEngine.Random.value;
        int prefabChoice = UnityEngine.Random.Range(0, projectilePrefabs.Length);

        if (spawnChance <= .10f && playerInRoom)
        {
            //Debug.Log("Spawn a trap projectile");

            SpawnProjectile(projectilePrefabs[prefabChoice]);

        }

        // now let's just move the traps we have to move too

        foreach(GameObject projectileObject in projectiles){
            ProjectileBehavior projectile = projectileObject.GetComponent<ProjectileBehavior>();

            projectile.Move();
        }
    }

    private void PlayerMovement_OnRoomEnter(object sender, PlayerMovement.InputArgs e)
    {
        if (e.roomId == roomId)
        {
            if (e.type == "enter")
            {
                //Debug.Log("player is in the same room as trap: " + gameObject.name);
                playerInRoom = true;
            }

            else
            {
                //Debug.Log("player left the room that trap: " + gameObject.name + " is in");
                playerInRoom = false;
            }

        }

    }


    private void SpawnProjectile(GameObject projectile)
    {

        Vector3 originPosition = transform.position;

        int randomSpawnChoice = UnityEngine.Random.Range(0, 8);

        Vector3 spawnPosition = originPosition;
        Vector3 direction = Vector3.zero;

        /*
            Spawn positions
                0 1 2
                3 X 4
                5 6 7
        */

        switch (randomSpawnChoice)
        {
            case 0:
                direction = Vector3.left + Vector3.up;
                spawnPosition = originPosition + direction;
                break;

            case 1:
                direction = Vector3.up;

                spawnPosition = originPosition + direction;
                break;

            case 2:
                direction = Vector3.right + Vector3.up;

                spawnPosition = originPosition + direction;
                break;

            case 3:
                direction = Vector3.left;

                spawnPosition = originPosition + direction;
                break;
            case 4:
                direction = Vector3.right;

                spawnPosition = originPosition + direction;
                break;
            case 5:
                direction = Vector3.down + Vector3.left;

                spawnPosition = originPosition + direction;
                break;
            case 6:
                direction = Vector3.down;

                spawnPosition = originPosition + direction;

                break;
            case 7:
                direction = Vector3.right + Vector3.down;

                spawnPosition = originPosition + direction;
                break;


            default:
                direction = Vector3.zero;
                spawnPosition = originPosition;
                break;
        }


        GameObject proj = Instantiate(projectile, spawnPosition, Quaternion.identity);
        proj.GetComponent<ProjectileBehavior>().directionOfTravel = direction;
        projectiles.Add(proj);


        return;
    }

}
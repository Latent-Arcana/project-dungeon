using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System;

public class LoreGeneration : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Object Prefabs")]

    [SerializeField]
    public GameObject[] loreObjects;

    Dictionary<Vector3Int, bool> placedObjects = new Dictionary<Vector3Int, bool>();

    public void Awake()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateLore(GameObject roomObj)
    {

        Room room = roomObj.GetComponent<Room>();


        // First let's figure out what kind of Lore room we are
        // The factory randomly chooses

        PopulateRoom(GetRandomRoomType(), room);


    }


    public void PopulateRoom(Enums.LoreRoomSubType subType, Room room)
    {

        // Get only the subtype objects we need
        List<GameObject> roomObjects = loreObjects.Where(x => x.GetComponent<LoreObjectBehavior>().SubType == subType).ToList();

        foreach (GameObject roomObject in roomObjects)
        {
            LoreObjectBehavior roomObjectBehavior = roomObject.GetComponent<LoreObjectBehavior>();

            Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

            PlacementRule placementRule = GetPlacementRuleByObject(roomObjectBehavior);

            if(placementRule.CanPlaceObject(tilemap, position, roomObjectBehavior.Width)){

                GameObject testObject = Instantiate(roomObject, position, Quaternion.identity);

                StartCoroutine(Check(testObject, room));
            }


            


            // PlacementRule placement = GetPlacementRuleByObject(roomObjectBehavior);

            // while (attempt <= 50 && numCreated < roomObjectBehavior.MaximumNumberAllowed)
            // {

            //     Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

            //     if (placement.CanPlaceObject(tilemap, room, placedObjects, position, roomObjectBehavior.IsWallSpawn))
            //     {

            //         Instantiate(roomObject, position, Quaternion.identity);
            //         // Place two entries for something 2x1
            //         placedObjects.Add(position, true);
            //         numCreated++;
            //     }

            //     else{
            //         ++attempt;
            //     }
            // }
        }


    }


    IEnumerator Check(GameObject testObject, Room room)
    {
        Collider2D collider = testObject.transform.GetChild(0).GetComponent<Collider2D>();

        LayerMask mask = 1 << LayerMask.NameToLayer("ObjectPlacementLayer");

        yield return new WaitForFixedUpdate();

        if(collider.IsTouchingLayers(mask)){

            Destroy(testObject);
        }
        else{
            testObject.transform.parent = room.gameObject.transform.GetChild(1).transform;
        }

    }

    private PlacementRule GetPlacementRuleByObject(LoreObjectBehavior loreObject)
    {

        switch (loreObject.PlacementType)
        {

            case Enums.PlacementType.Floor:
                return new FloorPlacementRule();

            case Enums.PlacementType.UpperWall:
                return new UpperWallPlacementRule();

            default:
                return new FloorPlacementRule();

        }
    }

    private Enums.LoreRoomSubType GetRandomRoomType()
    {

        int rand = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Enums.LoreRoomSubType)).Length);

        Enums.LoreRoomSubType loreRoomType = (Enums.LoreRoomSubType)rand;

        return loreRoomType;
    }
}
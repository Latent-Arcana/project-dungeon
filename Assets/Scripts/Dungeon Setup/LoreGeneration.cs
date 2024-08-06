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

            int attempt = 0;
            int numCreated = 0;

            PlacementRule placement = GetPlacementRuleByObject(roomObjectBehavior);

            while (attempt <= 50 && numCreated < roomObjectBehavior.MaximumNumberAllowed)
            {

                Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

                if (placement.CanPlaceObject(tilemap, room, placedObjects, position, roomObjectBehavior.IsWallSpawn))
                {

                    Instantiate(roomObject, position, Quaternion.identity);
                    placedObjects.Add(position, true);
                    numCreated++;
                }

                else{
                    ++attempt;
                }
            }
        }


    }

    private PlacementRule GetPlacementRuleByObject(LoreObjectBehavior loreObject)
    {

        switch (loreObject.ObjectType)
        {

            case Enums.ObjectType.Simple:
                return new SimplePlacementRule();

            case Enums.ObjectType.Wide:
                return new WidePlacementRule();

            default:
                return new SimplePlacementRule();

        }
    }

    private Enums.LoreRoomSubType GetRandomRoomType(){

        int rand = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Enums.LoreRoomSubType)).Length);

        Enums.LoreRoomSubType loreRoomType = (Enums.LoreRoomSubType)rand;

        return loreRoomType;
    }
}



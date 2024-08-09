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
            StartCoroutine(DoPlacementCheck(roomObject, room));
        }


    }


    IEnumerator DoPlacementCheck(GameObject roomObject, Room room)
    {
        LoreObjectBehavior roomObjectBehavior = roomObject.GetComponent<LoreObjectBehavior>();

        PlacementRule placementRule = GetPlacementRuleByObject(roomObjectBehavior);

        int attempt = 0;
        int numCreated = 0;

        while (attempt < 500 && numCreated < roomObjectBehavior.MaximumNumberAllowed)
        {

            Vector3Int position = placementRule.GetPointInRoom(room);

            if (placementRule.CanPlaceObject(tilemap, position, roomObjectBehavior.Width))
            {

                GameObject testObject = Instantiate(roomObject, position, Quaternion.identity);

                Collider2D collider = testObject.transform.GetChild(0).GetComponent<Collider2D>();

                LayerMask mask = 1 << LayerMask.NameToLayer("ObjectPlacementLayer");

                yield return new WaitForFixedUpdate();

                if (collider.IsTouchingLayers(mask))
                {
                    Destroy(testObject);
                }

                else
                {
                    testObject.transform.parent = room.gameObject.transform.GetChild(1).transform;
                    ++numCreated;
                }


            }

            attempt++;

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
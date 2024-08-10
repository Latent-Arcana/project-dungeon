using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System;

public class ObjectGeneration : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Object Prefabs")]

    [SerializeField]
    public GameObject[] loreObjects;

    public GameObject[] safeObjects;

    public GameObject[] dangerObjects;

    public GameObject[] unassignedObjects;

    Dictionary<Vector3Int, bool> placedObjects = new Dictionary<Vector3Int, bool>();

    public void Awake()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateObjectPlacements(List<GameObject> rooms)
    {
        foreach (GameObject roomObj in rooms)
        {

            Room room = roomObj.GetComponent<Room>();


            switch (room.roomType)
            {

                case Enums.RoomType.Safe:

                    PopulateRoom(safeObjects, room);

                    break;

                case Enums.RoomType.Danger:
                    PopulateRoom(dangerObjects, room);

                    break;

                case Enums.RoomType.Lore:

                    PopulateRoom(loreObjects, room);

                    break;

                default:
                    break;

            }

        }

    }


    public void PopulateRoom(GameObject[] objects, Room room)
    {

        // Only get subtype if we are dealing with lore rooms for now.
        // TODO: make this more scalable

        List<GameObject> roomObjects = new List<GameObject>();

        if (room.roomType == Enums.RoomType.Lore)
        {
            Enums.RoomSubType subType = GetRandomRoomSubType();
            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().RoomSubTypes.Contains(subType)).ToList();
        }

        else
        {
            roomObjects = objects.ToList();
        }

        // Randomly sort the list before using it so that we don't use the same objects in the same order every time

        roomObjects.Shuffle();

        foreach (GameObject roomObject in roomObjects)
        {
            StartCoroutine(DoPlacementCheck(roomObject, room));
        }


    }


    IEnumerator DoPlacementCheck(GameObject roomObject, Room room)
    {
        ObjectBehavior roomObjectBehavior = roomObject.GetComponent<ObjectBehavior>();

        PlacementRule placementRule = GetPlacementRuleByObject(roomObjectBehavior);

        int attempt = 0;
        int numCreated = 0;

        while (attempt < 100 && numCreated < roomObjectBehavior.MaximumNumberAllowed)
        {

            Vector3Int position = placementRule.GetPointInRoom(room);

            if (placementRule.CanPlaceObject(tilemap, position, roomObjectBehavior.Width, roomObjectBehavior.Height))
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

    private PlacementRule GetPlacementRuleByObject(ObjectBehavior loreObject)
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

    private Enums.RoomSubType GetRandomRoomSubType()
    {

        int rand = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Enums.RoomSubType)).Length);

        Enums.RoomSubType loreRoomType = (Enums.RoomSubType)rand;

        return loreRoomType;
    }
}
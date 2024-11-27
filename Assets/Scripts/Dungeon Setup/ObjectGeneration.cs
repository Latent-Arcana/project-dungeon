using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System;
using Unity.Collections;
using UnityEngine.AI;

public class ObjectGeneration : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Object Prefabs")]

    [SerializeField]
    public GameObject[] objects;


    public RoomSubType roomSubType_Bedroom;
    public RoomSubType roomSubType_Library;
    public RoomSubType roomSubType_Dining;
    public RoomSubType roomSubType_Armory;
    public RoomSubType roomSubType_Treasure;
    public RoomSubType roomSubType_TrapEasy;
    public RoomSubType roomSubType_TrapHard;
    public RoomSubType roomSubType_EnemyEasy;
    public RoomSubType roomSubType_EnemyHard;
    public RoomSubType roomSubType_Hospital;
    public RoomSubType roomSubType_Shrine;


    public GameObject portalPrefab;

    private int roomsRemaining;
    private int roomsCount;

    // This is how we're managing the count of unique instances of objects that have variants
    public ObjectCountManager objectCountManager;


    public static event Action AllRoomsPlacementComplete;
    public static event Action<float> RoomComplete;

    public void Awake()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateObjectPlacements(List<GameObject> rooms)
    {

        roomsRemaining = rooms.Count - 1;
        roomsCount = roomsRemaining;

        GeneratePortal(rooms);


        foreach (GameObject roomObj in rooms)
        {

            Room room = roomObj.GetComponent<Room>();

            RoomSubType currentSubType = null;


            switch (room.roomSubType)
            {
                case Enums.RoomSubType.Library:
                    currentSubType = roomSubType_Library;
                    break;

                case Enums.RoomSubType.Bedroom:
                    currentSubType = roomSubType_Bedroom;
                    break;

                case Enums.RoomSubType.Armory:
                    currentSubType = roomSubType_Armory;
                    break;

                case Enums.RoomSubType.Treasure:
                    currentSubType = roomSubType_Treasure;
                    break;

                case Enums.RoomSubType.Dining:
                    currentSubType = roomSubType_Dining;
                    break;

                case Enums.RoomSubType.TrapEasy:
                    currentSubType = roomSubType_TrapEasy;
                    break;

                case Enums.RoomSubType.TrapHard:
                    currentSubType = roomSubType_TrapHard;
                    break;

                case Enums.RoomSubType.EnemyEasy:
                    currentSubType = roomSubType_EnemyEasy;
                    break;

                case Enums.RoomSubType.EnemyHard:
                    currentSubType = roomSubType_EnemyHard;
                    break;

                case Enums.RoomSubType.Hospital:
                    currentSubType = roomSubType_Hospital;
                    break;

                case Enums.RoomSubType.Shrine:
                    currentSubType = roomSubType_Shrine;
                    break;

                default:
                    break;
            }


            if (currentSubType != null)
            {

                //Debug.Log(currentSubType.name + " in room " + roomObj.gameObject.name);

                StartCoroutine(PopulateRoom(room, currentSubType));

            }

        }

    }

    // We do portal generation first so that we don't end up with a room we can't spawn portals in
    public void GeneratePortal(List<GameObject> rooms)
    {
        List<int> portalRoomIndices = new List<int>();

        while (portalRoomIndices.Count < (rooms.Count / 5))
        {

            int randomPortalRoomIndex = UnityEngine.Random.Range(1, rooms.Count);
            if (!portalRoomIndices.Contains(randomPortalRoomIndex))
            {
                portalRoomIndices.Add(randomPortalRoomIndex);
            }
        }


        for (int i = 0; i < portalRoomIndices.Count; ++i)
        {
            int index = portalRoomIndices[i];

            Room room = rooms[i].GetComponent<Room>();

            Vector3 portalPosition = new Vector3(room.originX, room.originY, 0.0f);

            GameObject portal = GameObject.Instantiate(portalPrefab, portalPosition, Quaternion.identity);

            portal.transform.SetParent(room.gameObject.transform);

            //Debug.Log("Placed portal in room: " + index);
        }

    }


    public IEnumerator PopulateRoom(Room room, RoomSubType currentSubType)
    {

        Dictionary<Enums.ObjectType, int> placedObjects = new Dictionary<Enums.ObjectType, int>();

        List<GameObject> roomObjects = new List<GameObject>();

        // make sure all required objects get placed
        foreach (Enums.ObjectType objectType in currentSubType.RequiredObjects)
        {

            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().ObjectType == objectType).ToList();

            yield return StartCoroutine(DoPlacementChecks(roomObjects, room, currentSubType, objectType, placedObjects));

        }

        // handle decor objects
        int randomItemMax = UnityEngine.Random.Range(5, 10);

        for (int i = 0; i < randomItemMax; i++)
        {

            int randomDecorItemIndex = UnityEngine.Random.Range(0, currentSubType.DecorObjects.Count);

            Enums.ObjectType objectType = currentSubType.DecorObjects[randomDecorItemIndex];

            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().ObjectType == objectType).ToList();

            yield return StartCoroutine(DoPlacementChecks(roomObjects, room, currentSubType, objectType, placedObjects));

        }

        Debug.Log("--------------" + room.roomId + "-----------");

        foreach(KeyValuePair<Enums.ObjectType, int> pair in placedObjects){
            Debug.Log("Key: " + pair.Key + "  Value: " + pair.Value);
        }

        roomsRemaining--;

        RoomComplete?.Invoke((float)(roomsCount - roomsRemaining) / (float)roomsCount);

        if (roomsRemaining <= 0)
        {

            AllRoomsPlacementComplete?.Invoke();

        }


    }


    IEnumerator DoPlacementChecks(List<GameObject> roomObjectsOfType, Room room, RoomSubType currentRoomSubType, Enums.ObjectType objectType, Dictionary<Enums.ObjectType, int> placedObjects)
    {

        int randomObjectIndex = UnityEngine.Random.Range(0, roomObjectsOfType.Count);


        GameObject roomObject = roomObjectsOfType[randomObjectIndex];
        ObjectBehavior roomObjectBehavior = roomObject.GetComponent<ObjectBehavior>();
        PlacementRule placementRule = GetPlacementRuleByObject(roomObjectBehavior);

        int attempt = 0;

        // if the placedObjects list contains the key, but its value for this object type is less than the max allowed
        if ((placedObjects.ContainsKey(objectType) && placedObjects[objectType] < currentRoomSubType.MaxAllowed[objectType]) || !placedObjects.ContainsKey(objectType))
        {

            //Debug.Log($"Entering while loop to attempt to place {roomObjectBehavior.gameObject.name}");

            while (attempt < 100)
            {

                // generate a point and return it if it's valid. otherwise return Vector3Int.zero
                Vector3Int position = placementRule.CanPlaceObject(tilemap, room, roomObjectBehavior.Width, roomObjectBehavior.Height);

                // If we can place an object at the point we selected
                if (position != Vector3Int.zero)
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

                        // Let's do our test on the sprite to make sure it's flipped in the correct direction
                        SideWallPlacementRule sideWallPlacement = placementRule as SideWallPlacementRule;

                        if (sideWallPlacement != null)
                        {
                            Tuple<bool, bool> leftOrRight = sideWallPlacement.LeftOrRightWall(tilemap, position, roomObjectBehavior.Width, roomObjectBehavior.Height);

                            if (!leftOrRight.Item1 && leftOrRight.Item2)
                            { // The objects are all left by default, so if we're on the right side wall we flip
                                if (roomObjectBehavior.flippedSprite != null)
                                {
                                    testObject.GetComponent<SpriteRenderer>().sprite = roomObjectBehavior.flippedSprite;
                                }
                            }

                        }

                        testObject.transform.parent = room.gameObject.transform.GetChild(1).transform;

                        // in the case that we don't have an entry for this key we just have to set it to 1 instead of ++
                        if (placedObjects.ContainsKey(objectType))
                        {
                            ++placedObjects[objectType];

                        }

                        else
                        {
                            placedObjects[objectType] = 1;
                        }

                        attempt += 100; // set attempt beyond our max attempt list

                    }

                }

                attempt++;

            }
        }

    }



    private PlacementRule GetPlacementRuleByObject(ObjectBehavior roomObject)
    {

        switch (roomObject.PlacementType)
        {

            case Enums.PlacementType.Floor:
                return new FloorPlacementRule();

            case Enums.PlacementType.UpperWall:
                return new UpperWallPlacementRule();

            case Enums.PlacementType.SideWall:
                return new SideWallPlacementRule();

            default:
                return new FloorPlacementRule();

        }
    }

}
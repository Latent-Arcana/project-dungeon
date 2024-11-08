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

    // public GameObject[] safeObjects;

    // public GameObject[] dangerObjects;

    // public GameObject[] unassignedObjects;

    public RoomSubType roomSubType_Bedroom;
    public RoomSubType roomSubType_Library;

    public GameObject portalPrefab;

    private int roomsRemaining;
    private int roomsCount;


    Dictionary<Enums.ObjectType, int> placedObjects = new Dictionary<Enums.ObjectType, int>();

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

                default:
                    break;
            }


            if (currentSubType != null)
            {

                Debug.Log(currentSubType.name + " in room " + roomObj.gameObject.name);

                StartCoroutine(PopulateRoom(room, currentSubType));

            }

        }

    }


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

            Debug.Log("Placed portal in room: " + index);
        }

    }


    public IEnumerator PopulateRoom(Room room, RoomSubType currentSubType)
    {

        List<GameObject> roomObjects = new List<GameObject>();

        // make sure all required objects get placed
        foreach (Enums.ObjectType objectType in currentSubType.RequiredObjects)
        {

            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().ObjectType == objectType).ToList();

            yield return StartCoroutine(DoPlacementChecks(roomObjects, room, currentSubType, objectType));

        }

        // handle decor objects
        int randomItemMax = UnityEngine.Random.Range(5, 10);

        for (int i = 0; i < randomItemMax; i++)
        {

            int randomDecorItemIndex = UnityEngine.Random.Range(0, currentSubType.DecorObjects.Count);

            Enums.ObjectType objectType = currentSubType.DecorObjects[randomDecorItemIndex];

            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().ObjectType == objectType).ToList();

            yield return StartCoroutine(DoPlacementChecks(roomObjects, room, currentSubType, objectType));

        }








        roomsRemaining--;

        //Debug.Log("a room finished its placement. There are " + roomsRemaining + " rooms left");

        RoomComplete?.Invoke((float)(roomsCount - roomsRemaining) / (float)roomsCount);

        if (roomsRemaining <= 0)
        {

            AllRoomsPlacementComplete?.Invoke();

        }


    }


    IEnumerator DoPlacementChecks(List<GameObject> roomObjectsOfType, Room room, RoomSubType currentRoomSubType, Enums.ObjectType objectType)
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
        // if it doesn't contain the key (implying this is our first attempt at this object type)



        // foreach (GameObject roomObject in roomObjects)
        // {

        //     ObjectBehavior roomObjectBehavior = roomObject.GetComponent<ObjectBehavior>();

        //     PlacementRule placementRule = GetPlacementRuleByObject(roomObjectBehavior);

        //     Enums.ObjectType objectType = roomObjectBehavior.ObjectType;

        //     int attempt = 0;
        //     int maxAllowed = objectCountManager.GetCountAllowedByObjectType(objectType);
        //     // max is some amount between 1 and the max allowed of the object type
        //     int max = GetRandomNumberOfObjects(maxAllowed);

        //     int numCreated = objectCounter.GetCountByType(objectType);

        //     // if (roomObjectBehavior.ObjectType == Enums.ObjectType.Glass)
        //     // {
        //     //     Debug.Log("we can create: " + numCreated);
        //     // }

        //     while (attempt < 100 && numCreated < max)
        //     {

        //         // generate a point and return it if it's valid. otherwise return Vector3Int.zero
        //         Vector3Int position = placementRule.CanPlaceObject(tilemap, room, roomObjectBehavior.Width, roomObjectBehavior.Height);

        //         // If we can place an object at the point we selected
        //         if (position != Vector3Int.zero)
        //         {

        //             // if (roomObjectBehavior.ObjectType == Enums.ObjectType.Glass)
        //             // {
        //             //     Debug.Log("attempting to place glass");
        //             // }

        //             GameObject testObject = Instantiate(roomObject, position, Quaternion.identity);

        //             Collider2D collider = testObject.transform.GetChild(0).GetComponent<Collider2D>();

        //             LayerMask mask = 1 << LayerMask.NameToLayer("ObjectPlacementLayer");

        //             yield return new WaitForFixedUpdate();

        //             if (collider.IsTouchingLayers(mask))
        //             {
        //                 Destroy(testObject);
        //             }

        //             else
        //             {
        //                 testObject.transform.parent = room.gameObject.transform.GetChild(1).transform;
        //                 ++numCreated;
        //                 objectCounter.IncreaseCountByType(objectType, 1);

        //                 // if (roomObjectBehavior.ObjectType == Enums.ObjectType.Glass)
        //                 // {
        //                 //     Debug.Log("placed glass");
        //                 // }
        //             }


        //         }

        //         attempt++;

        //     }

        // }

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

    private int GetRandomNumberOfObjects(int maxAllowed)
    {

        return UnityEngine.Random.Range(1, maxAllowed + 1);
    }

    private Enums.RoomSubType GetRandomRoomSubType()
    {

        int rand = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Enums.RoomSubType)).Length);

        Enums.RoomSubType subType = (Enums.RoomSubType)rand;

        return subType;
    }


    public class ObjectCounts
    {
        // Running totals by object type
        public int bedCount;
        public int candleCount;
        public int chestCount;
        public int debrisCount;
        public int bookShelfCount;
        public int armorStandCount;
        public int tableCount;
        public int chairCount;
        public int spikesCount;

        public ObjectCounts()
        {
            bedCount = 0;
            candleCount = 0;
            chestCount = 0;
            debrisCount = 0;
            bookShelfCount = 0;
            armorStandCount = 0;
            tableCount = 0;
            chairCount = 0;
            spikesCount = 0;
        }

        public void IncreaseCountByType(Enums.ObjectType objectType, int count)
        {
            if (objectType == Enums.ObjectType.Bed) { bedCount += count; }
            else if (objectType == Enums.ObjectType.Bookshelf) { bookShelfCount += count; }
            else if (objectType == Enums.ObjectType.Candle) { candleCount += count; }
            else if (objectType == Enums.ObjectType.Chest) { chestCount += count; }
            else if (objectType == Enums.ObjectType.Debris) { debrisCount += count; }
            else if (objectType == Enums.ObjectType.ArmorStand) { armorStandCount += count; }
            else if (objectType == Enums.ObjectType.Table) { tableCount += count; }
            else if (objectType == Enums.ObjectType.Chair) { chairCount += count; }
            else if (objectType == Enums.ObjectType.Spikes) { spikesCount += count; }
            else
            {
                return;
            }
        }


        public int GetCountByType(Enums.ObjectType objectType)
        {
            if (objectType == Enums.ObjectType.Bed) { return bedCount; }
            else if (objectType == Enums.ObjectType.Bookshelf) { return bookShelfCount; }
            else if (objectType == Enums.ObjectType.Candle) { return candleCount; }
            else if (objectType == Enums.ObjectType.Chest) { return chestCount; }
            else if (objectType == Enums.ObjectType.Debris) { return debrisCount; }
            else if (objectType == Enums.ObjectType.ArmorStand) { return armorStandCount; }
            else if (objectType == Enums.ObjectType.Table) { return tableCount; }
            else if (objectType == Enums.ObjectType.Chair) { return chairCount; }
            else if (objectType == Enums.ObjectType.Spikes) { return spikesCount; }

            else
            {
                return 1000; // huge number if we don't actually have a maximum defined
            }
        }

    }

}
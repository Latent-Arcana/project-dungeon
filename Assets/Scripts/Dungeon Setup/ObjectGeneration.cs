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
    public GameObject[] loreObjects;

    public GameObject[] safeObjects;

    public GameObject[] dangerObjects;

    public GameObject[] unassignedObjects;

    public GameObject portalPrefab;

    private int roomsRemaining;
    private int roomsCount;


    Dictionary<Vector3Int, bool> placedObjects = new Dictionary<Vector3Int, bool>();

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

            switch (room.roomType)
            {

                case Enums.RoomType.Safe:

                    StartCoroutine(PopulateRoom(safeObjects, room));
                    break;

                case Enums.RoomType.Danger:
                    StartCoroutine(PopulateRoom(dangerObjects, room));

                    break;

                case Enums.RoomType.Lore:

                    StartCoroutine(PopulateRoom(loreObjects, room));

                    break;

                default:

                    //Debug.Log("We hit the default room switch case in room " + room.roomId);
                    //PopulateRoom(safeObjects, room);

                    break;

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


    public IEnumerator PopulateRoom(GameObject[] objects, Room room)
    {

        // Only get subtype if we are dealing with lore rooms for now.
        // TODO: make this more scalable

        List<GameObject> roomObjects = new List<GameObject>();

        Enums.RoomSubType subType = GetRandomRoomSubType();

        if (room.roomType == Enums.RoomType.Lore)
        {
            //Debug.Log(room.roomId + " is a " + room.roomType + " of subtype: " + subType);
            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().RoomSubTypes.Contains(subType)).ToList();
        }

        else if (room.roomType == Enums.RoomType.Danger)
        {
            roomObjects = objects.Where(x => x.GetComponent<ObjectBehavior>().RoomSubTypes.Contains(subType)).ToList();
        }

        else
        {
            roomObjects = objects.ToList();
        }

        Debug.Log("Room " + room.roomId + " is a " + room.roomType + " Room of subtype " + subType.ToString());

        // Randomly sort the list before using it so that we don't use the same objects in the same order every time


        // TODO:
        // Go keep track of maxCount outside each individual room object. Otherwise we just start over every time and get the same count each time

        roomObjects.Shuffle();

        ObjectCounts objectCounter = new ObjectCounts();


        yield return StartCoroutine(DoPlacementChecks(roomObjects, room));

        roomsRemaining--;
        
        Debug.Log("a room finished its placement. There are " + roomsRemaining + " rooms left");
        
        RoomComplete?.Invoke((float)(roomsCount - roomsRemaining) / (float)roomsCount);
       
        if(roomsRemaining <= 0){

            AllRoomsPlacementComplete?.Invoke();
        
        }


    }


    IEnumerator DoPlacementChecks(List<GameObject> roomObjects, Room room)
    {

        ObjectCounts objectCounter = new ObjectCounts();

        foreach (GameObject roomObject in roomObjects)
        {

            ObjectBehavior roomObjectBehavior = roomObject.GetComponent<ObjectBehavior>();

            PlacementRule placementRule = GetPlacementRuleByObject(roomObjectBehavior);

            Enums.ObjectType objectType = roomObjectBehavior.ObjectType;

            int attempt = 0;
            int maxAllowed = objectCountManager.GetCountAllowedByObjectType(objectType);
            // max is some amount between 1 and the max allowed of the object type
            int max = GetRandomNumberOfObjects(maxAllowed);

            int numCreated = objectCounter.GetCountByType(objectType);

            // if (roomObjectBehavior.ObjectType == Enums.ObjectType.Glass)
            // {
            //     Debug.Log("we can create: " + numCreated);
            // }

            while (attempt < 100 && numCreated < max)
            {

                // generate a point and return it if it's valid. otherwise return Vector3Int.zero
                Vector3Int position = placementRule.CanPlaceObject(tilemap, room, roomObjectBehavior.Width, roomObjectBehavior.Height);

                // If we can place an object at the point we selected
                if (position != Vector3Int.zero)
                {

                    // if (roomObjectBehavior.ObjectType == Enums.ObjectType.Glass)
                    // {
                    //     Debug.Log("attempting to place glass");
                    // }

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
                        objectCounter.IncreaseCountByType(objectType, 1);

                        // if (roomObjectBehavior.ObjectType == Enums.ObjectType.Glass)
                        // {
                        //     Debug.Log("placed glass");
                        // }
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
        public int glassCount;

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
            glassCount = 0;
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
            else if (objectType == Enums.ObjectType.Glass) { glassCount += count; }
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
            else if (objectType == Enums.ObjectType.Glass) { return glassCount; }

            else
            {
                return 1000; // huge number if we don't actually have a maximum defined
            }
        }

    }

}
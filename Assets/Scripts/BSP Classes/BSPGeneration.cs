using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static PlayerMovement;
using static BSPGeneration;

public class BSPGeneration : MonoBehaviour
{

    [Header("Dungeon Parameters")]
    public int mapWidth;
    public int mapHeight;
    public int roomMinimumWidth;
    public int roomMinimumHeight;

    public int mapOffset = 500;

    [Header("Dungeon Tiles")]

    [SerializeField]
    private Tile gameplayFloorTile;// Dungeon Floor and Dungeon Wall Tiles

    [SerializeField]
    private Tile gameplayFloorHallwayTile;

    [SerializeField]
    private RuleTile gameplayWallTile;


    [SerializeField]
    private RuleTile mapFloorAndWallTile;

    [SerializeField]
    private Tile mapOutsideTile;


    [SerializeField]
    private GameObject fogGameplayPrefab;

    public event EventHandler<BSPArgs> OnBSPFinished;

     public class BSPArgs : EventArgs
    {
       public List<GameObject> rooms;
    }

    
    
    [Header("Other Stuff")]
    public Tilemap mainTilemap;
    //public Tilemap mapTilemap;

    public List<GameObject> allRooms = new List<GameObject>();

    public float[][] distances;

    public GameObject player;

    public DangerGeneration Danger_Generator;
    public LoreGeneration Lore_Generator;

    public Partition dungeon;

    void Awake()
    {
        // Get the tilemaps
        mainTilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
        //mapTilemap = GameObject.Find("Map Tilemap").GetComponent<Tilemap>();

        // Get Dungeon Setup
        Danger_Generator = gameObject.GetComponent<DangerGeneration>();
        Lore_Generator = gameObject.GetComponent<LoreGeneration>();
        // Get Enemy Generator
        dangerGenerator = gameObject.GetComponent<DangerGeneration>();

        // Get the player so we can place them at the correct location
        player = GameObject.Find("Player");

        // let's make the entire background walls
        for (int i = -25; i < 75; ++i)
        {
            for (int j = -25; j < 75; ++j)
            {
                mainTilemap.SetTile(new Vector3Int(i, j, 0), gameplayWallTile);

                mainTilemap.SetTile(new Vector3Int(i - mapOffset, j - mapOffset, 0), mapOutsideTile);
            }
        }

        // Let's create the dungeon's bounds
        dungeon = new Partition(0, 0, mapWidth, mapHeight, roomMinimumHeight); // passing minimum height here because room takes "size" and we have both height and width

        BSP(dungeon);

        DrawPartitionsAndCreateRooms(dungeon); // doing these two things together to do it all in one pass over the tree


        //initializes an array of arrays to hold the distances 
        distances = new float[allRooms.Count][];

        for (int i = 0; i < allRooms.Count; i++)
        {
            distances[i] = new float[allRooms.Count];
        }

        // Draw the rooms and set their types
        for (int i = 0; i < allRooms.Count; i++)
        {


            Room room1 = allRooms[i].GetComponent<Room>();
            DrawRoom(room1);


            // calculating distances between all rooms
            for (int j = allRooms[i].GetComponent<Room>().roomId; j < allRooms.Count; j++)
            {

                Room room2 = allRooms[j].GetComponent<Room>();

                Vector3Int room1Origin = new Vector3Int(room1.originX, room1.originY, 0);
                Vector3Int room2Origin = new Vector3Int(room2.originX, room2.originY, 0);
                distances[room1.roomId][j] = Vector3Int.Distance(room1Origin, room2Origin);

            }

        }

        CreateCorridors(dungeon);

        // now we can place our player at the start of the dungeon.
        // FOR NOW WE ARE PICKING THE FIRST ROOM IN THE LIST, ARBITRARILY

        Player_Movement.gameObject.transform.position = new Vector3Int(allRooms[0].GetComponent<Room>().originX, allRooms[0].GetComponent<Room>().originY, 0);

        // Time to set up the dungeon
         // Set Types
            // Safe Room - 20 %

            // Lore Room - 40 %
            // 2 subtypes (Unassigned 25% and Interactive 75%)

            // Danger Room - 40 %
            // 4 subtypes


        // for every room, let's tag it and populate it!
        for(int i = 0; i < allRooms.Count; ++i){

            Room room = allRooms[i].GetComponent<Room>();

            int rand = UnityEngine.Random.Range(0, 100);

            if (rand < 20) // Safe
            {
                room.roomType = Enums.RoomType.Safe;
            }

            else if (rand < 60) // Lore
            {
                if (rand > 30)
                {
                    room.roomType = Enums.RoomType.Lore;

                    Lore_Generator.GenerateLore(allRooms[i]);
                }

                // otherwise, room stays as Unassigned (initialized that way)

            }

            else // Danger
            {
                if (i > 0)
                {
                    // Debug.Log("Room " + allRooms[i].roomId + " is a danger room defined at: (" + allRooms[i].x + ", " + allRooms[i].y + ")");
                    room.roomType = Enums.RoomType.Danger;
                    // Place Enemies
                    Danger_Generator.GenerateEnemies(allRooms[i]);
                }
            }
        }

    }


        }

        CreateCorridors(dungeon);

        // now we can place our player at the start of the dungeon.
        // FOR NOW WE ARE PICKING THE FIRST ROOM IN THE LIST, ARBITRARILY

        player.transform.position = new Vector3Int(allRooms[0].GetComponent<Room>().originX, allRooms[0].GetComponent<Room>().originY, 0);


    }

    //private void OnDrawGizmos()
    //{
    //    foreach (Room room in allRooms)
    //    {
    //        Gizmos.DrawCube(room.roomObject.transform.position, room.roomObject.transform.localScale);
    //    }
    //}

    void BSP(Partition part)
    {
        Tuple<Partition, Partition> splitRooms;

        //the new room is too long
        if (part.width / part.height >= 2)
        {
            splitRooms = Split(part, Enums.PartitionType.LeftRight);
        }

        //the new room is too tall
        else if (part.height / part.width >= 2)
        {
            splitRooms = Split(part, Enums.PartitionType.TopBottom);
        }

        //normal random split
        else
        {
            splitRooms = Split(part, Enums.PartitionType.Random);
        }

        // keep in mind left and right child is just a naming convention for the children nodes in our binary tree

        if (splitRooms.Item1.width >= roomMinimumWidth + 1 && splitRooms.Item1.height >= roomMinimumHeight + 1 && splitRooms.Item2.width >= roomMinimumWidth + 1 && splitRooms.Item2.height >= roomMinimumHeight + 1) // using + 2 as padding to protect us from overlapping rooms inside adjacent partitions
        {
            part.leftChild = splitRooms.Item1;
            part.rightChild = splitRooms.Item2;

            BSP(part.leftChild);
            BSP(part.rightChild);
        }

        else
        {
            part.isLeaf = true;
        }

    }

    Tuple<Partition, Partition> Split(Partition partition, Enums.PartitionType type)
    {
        if (type == Enums.PartitionType.TopBottom)
        {
            int rngOffset = partition.height / 2;

            Partition bottom = new Partition(partition.x, partition.y, partition.width, rngOffset, roomMinimumHeight);
            Partition top = new Partition(partition.x, partition.y + rngOffset, partition.width, partition.height - rngOffset, roomMinimumHeight);

            // Debug.Log($"Force a Top/Bottom split at ({partition.x},{partition.y}) with new parts at ({top.x},{top.y}) adn ({bottom.x},{bottom.y})");

            Tuple<Partition, Partition> splitRooms = Tuple.Create(top, bottom);

            return splitRooms;
        }

        else if (type == Enums.PartitionType.LeftRight)
        {
            int rngOffset = partition.width / 2;

            Partition left = new Partition(partition.x, partition.y, rngOffset, partition.height, roomMinimumHeight);
            Partition right = new Partition(partition.x + rngOffset, partition.y, partition.width - rngOffset, partition.height, roomMinimumHeight);

            //  Debug.Log($"Force a Left/Right split {partition.x},{partition.y} with new parts at ({left.x},{left.y}) adn ({right.x},{right.y})");

            Tuple<Partition, Partition> splitRooms = Tuple.Create(left, right);

            return splitRooms;
        }

        else
        {
            // randomly decide a direction in which to slice
            int rngDirection = UnityEngine.Random.Range(0, 2); // int 0 OR 1

            // Horizontal Plane
            if (rngDirection == 0)
            {
                // randomly decide location on horizontal plane
                int rngOffset = UnityEngine.Random.Range(roomMinimumHeight + 2, partition.height - roomMinimumHeight);

                Partition bottom = new Partition(partition.x, partition.y, partition.width, rngOffset, roomMinimumHeight);
                Partition top = new Partition(partition.x, partition.y + rngOffset, partition.width, partition.height - rngOffset, roomMinimumHeight);

                //   Debug.Log($"Random split {partition.x},{partition.y}");

                Tuple<Partition, Partition> splitRooms = Tuple.Create(top, bottom);

                return splitRooms;
            }

            // Vertical Plane
            else
            {
                // randomly decide location on vertical plane
                int rngOffset = UnityEngine.Random.Range(roomMinimumWidth + 2, partition.width - roomMinimumWidth);

                Partition left = new Partition(partition.x, partition.y, rngOffset, partition.height, roomMinimumHeight);
                Partition right = new Partition(partition.x + rngOffset, partition.y, partition.width - rngOffset, partition.height, roomMinimumHeight);

                //   Debug.Log($"Random split {partition.x},{partition.y}");


                Tuple<Partition, Partition> splitRooms = Tuple.Create(left, right);

                return splitRooms;
            }
        }

    }

    void CreateCorridors(Partition root)
    {
        if (root == null) return;

        if (root.leftChild != null && root.rightChild != null)
        {
            CreateCorridors(root.leftChild);
            CreateCorridors(root.rightChild);

            // Connect the clusters formed by the child partitions
            CreateCorridorBetweenPartitions(root.leftChild, root.rightChild);
        }
    }

    void CreateCorridorBetweenPartitions(Partition first, Partition second)
    {
        if (first.isLeaf && second.isLeaf)
        {
            Room firstRoom = allRooms[first.roomId].GetComponent<Room>();
            Room secondRoom = allRooms[second.roomId].GetComponent<Room>();
            DrawCorridor(firstRoom, secondRoom);
        }

        else if (first.isLeaf)
        {
            // Find a leaf node in second and connect to first's room
            Room firstRoom = allRooms[first.roomId].GetComponent<Room>();
            Room secondRoom = FindClosestRoom(second, firstRoom);
            if (secondRoom != null)
            {
                DrawCorridor(firstRoom, secondRoom);
            }
        }
        else if (second.isLeaf)
        {
            // Find a leaf node in first and connect to second's room
            Room secondRoom = allRooms[second.roomId].GetComponent<Room>();
            Room firstRoom = FindClosestRoom(first, secondRoom);

            if (firstRoom != null)
            {
                DrawCorridor(firstRoom, secondRoom);
            }
        }
        else
        {

            List<Room> firstRooms = GetChildRooms(first);
            List<Room> secondRooms = GetChildRooms(second);

            float minDistance = float.MaxValue;

            Room firstRoomClosest = null;
            Room secondRoomClosest = null;

            foreach (Room firstRoom in firstRooms)
            {
                foreach (Room secondRoom in secondRooms)
                {

                    Vector3Int firstRoomPosition = new Vector3Int(firstRoom.originX, firstRoom.originY, 0);
                    Vector3Int secondRoomPosition = new Vector3Int(secondRoom.originX, secondRoom.originY, 0);

                    float calculatedDistance;

                    if (firstRoom.roomId < secondRoom.roomId)
                    {
                        calculatedDistance = distances[firstRoom.roomId][secondRoom.roomId];

                    }
                    else
                    {
                        calculatedDistance = distances[secondRoom.roomId][firstRoom.roomId];

                    }


                    if (calculatedDistance <= minDistance && calculatedDistance != 0)
                    {
                        minDistance = calculatedDistance;
                        firstRoomClosest = firstRoom;
                        secondRoomClosest = secondRoom;
                    }
                }
            }

            if (firstRoomClosest != null && secondRoomClosest != null)
            {
                DrawCorridor(firstRoomClosest, secondRoomClosest);
            }
        }
    }

    List<Room> GetChildRooms(Partition partition)
    {
        List<Room> rooms = new List<Room>();
        if (partition == null) return rooms;

        if (partition.isLeaf)
        {
            rooms.Add(allRooms[partition.roomId].GetComponent<Room>());
        }
        else
        {
            if (partition.leftChild != null)
            {
                rooms.AddRange(GetChildRooms(partition.leftChild));
            }
            if (partition.rightChild != null)
            {
                rooms.AddRange(GetChildRooms(partition.rightChild));
            }
        }

        return rooms;
    }

    Room FindClosestRoom(Partition root, Room start)
    {
        Queue<Partition> queue = new Queue<Partition>();

        queue.Enqueue(root);

        Room closestRoom = null;

        Vector3Int startPoint = new Vector3Int(start.originX, start.originY, 0);

        float minDistance = float.MaxValue;

        while (queue.Count > 0)
        {

            Partition partition = queue.Peek();

            if (partition.leftChild != null)
            {
                queue.Enqueue(partition.leftChild);
            }

            if (partition.rightChild != null)
            {
                queue.Enqueue(partition.rightChild);
            }

            if (partition.isLeaf)
            {
                if (distances[start.roomId][partition.roomId] < minDistance && distances[start.roomId][partition.roomId] != 0)
                {
                    minDistance = distances[start.roomId][partition.roomId];
                    closestRoom = allRooms[partition.roomId].GetComponent<Room>();
                }
            }
            queue.Dequeue();
        }

        return closestRoom;
    }

    void DrawCorridor(Room room1, Room room2)
    {
        Vector3Int start = new Vector3Int(room1.originX, room1.originY, 0);
        Vector3Int end = new Vector3Int(room2.originX, room2.originY, 0);

        DrawCooridorX(ref start, ref end, out Vector2Int horizontalStart, out Vector2Int horizontalEnd);
        DrawCooridorY(ref start, ref end, out Vector2Int verticalStart, out Vector2Int verticalEnd);


        //Game Object Stuff
        GameObject hallwayObject = new GameObject("Hallway_" + room1.roomId.ToString() + "_" + room2.roomId.ToString());
        //GameObject hallwayObjGameplay = new GameObject("Hallway_Gameplay");

        //parent objects
        hallwayObject.transform.SetParent(GameObject.Find("Hallways").transform);
        //hallwayObjGameplay.transform.SetParent(hallwayObject.transform); //child 0
        //no map object yet, but we could make one if we fog the map differently

        //attach hallway class and assign
        Hallway hallway = hallwayObject.AddComponent<Hallway>();
        hallway.horizontalStart = horizontalStart;
        hallway.horizontalEnd = horizontalEnd;
        hallway.verticalStart = verticalStart;
        hallway.verticalEnd = verticalEnd;
        hallway.room1Id = room1.roomId;
        hallway.room2Id = room2.roomId;

        //Tag
        hallwayObject.tag = "hallway";

        //
        hallwayObject.AddComponent<HallwayFogController>();

        //  Horizontal Collider
        BoxCollider2D horizCollider = hallwayObject.AddComponent<BoxCollider2D>();
        horizCollider.isTrigger = true;
        horizCollider.offset = new Vector2(
            horizontalStart.x + .5f + (horizontalEnd.x - horizontalStart.x) / 2f,
            horizontalStart.y + 0.5f
            ); //offset is centered
        horizCollider.size = new Vector2(Mathf.Max(horizontalEnd.x - horizontalStart.x - 0.25f, .5f), 0.5f); // for single tile hallways, x-x is 0 so we have to take the max of 0.5f

        //  Vertical Collider
        BoxCollider2D vertCollider = hallwayObject.AddComponent<BoxCollider2D>();
        vertCollider.isTrigger = true;
        vertCollider.offset = new Vector2(
            verticalStart.x + 0.5f,
            verticalStart.y + .5f + (verticalEnd.y - verticalStart.y) / 2f
            ); //offset is centered
        vertCollider.size = new Vector2(0.5f, Mathf.Max(verticalEnd.y - verticalStart.y - 0.25f, .5f)); // for single tile hallways, y-y is 0 so we have to take the max of 0.5f


        //horizontal line object
        if (horizontalStart != Vector2Int.zero)
        {
            GameObject fogBoxHorizontal = Instantiate(fogGameplayPrefab, new Vector3Int(horizontalStart.x, horizontalStart.y, 0), Quaternion.identity);
            fogBoxHorizontal.transform.localScale = new Vector3(horizontalEnd.x - horizontalStart.x + 1, 1, 0);
            fogBoxHorizontal.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");
            fogBoxHorizontal.transform.SetParent(hallwayObject.transform);
            fogBoxHorizontal.name = "Horizontal Hallway";

            //fogBoxHorizontal.AddComponent<HallwayFogController>();
            fogBoxHorizontal.tag = "hallway";
        }

        //vertical line object 
        if (verticalStart != Vector2Int.zero)
        {
            GameObject fogBoxVertical = Instantiate(fogGameplayPrefab, new Vector3Int(verticalStart.x, verticalStart.y, 0), Quaternion.identity);
            fogBoxVertical.transform.localScale = new Vector3(1, verticalEnd.y - verticalStart.y + 1, 0);
            fogBoxVertical.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");
            fogBoxVertical.transform.SetParent(hallwayObject.transform);
            fogBoxVertical.name = "Vertical Hallway";

            // BoxCollider2D roomColl = fogBoxVertical.AddComponent<BoxCollider2D>();
            // roomColl.isTrigger = true;

            //fogBoxVertical.AddComponent<HallwayFogController>();
            fogBoxVertical.tag = "hallway";
        }

    }

    public void DrawCooridorX(ref Vector3Int start, ref Vector3Int end, out Vector2Int hallStart, out Vector2Int hallEnd)
    {
        hallStart = new Vector2Int(0, 0);
        hallEnd = new Vector2Int(0, 0);
        bool isSet = false;

        //Debug.Log($"Checking location ({start.x}, {start.y}) - {mainTilemap.GetTile(start)} ");


        if (start.x < end.x) //from left to right
        {
            while (start.x != end.x)
            {
                start.x += 1;

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // only draw hallways over
                {
                    if (!isSet) //set the starting position one time
                    {
                        isSet = true;
                        hallStart.x = start.x;
                        hallStart.y = start.y;
                    }
                    hallEnd.x = start.x;
                    hallEnd.y = start.y;

                    //Set the tile to flooring
                    mainTilemap.SetTileFlags(start, TileFlags.None);
                    mainTilemap.SetTile(start, gameplayFloorHallwayTile);

                    mainTilemap.SetTileFlags(start + new Vector3Int(-mapOffset, -mapOffset, 0), TileFlags.None);
                    mainTilemap.SetTile(start + new Vector3Int(-mapOffset, -mapOffset, 0), mapFloorAndWallTile);

                }
            }
        }

        else if (start.x > end.x)//from right to left
        {
            while (start.x != end.x)
            {
                start.x -= 1;

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already
                {
                    if (!isSet) //set the starting position one time
                    {
                        isSet = true;
                        hallEnd.x = start.x; //this makes the "start" hallway to the left of the "end" hallway
                        hallEnd.y = start.y;
                    }
                    hallStart.x = start.x;
                    hallStart.y = start.y;

                    //Set the tile to flooring
                    mainTilemap.SetTileFlags(start, TileFlags.None);
                    mainTilemap.SetTile(start, gameplayFloorHallwayTile);

                    mainTilemap.SetTileFlags(start + new Vector3Int(-mapOffset, -mapOffset, 0), TileFlags.None);
                    mainTilemap.SetTile(start + new Vector3Int(-mapOffset, -mapOffset, 0), mapFloorAndWallTile);
                }
            }
        }

        //Debug.Log($"Hallway from ({hallStart.x},{hallStart.y}) to ({hallEnd.x},{hallEnd.y})");

        return;
    }

    public void DrawCooridorY(ref Vector3Int start, ref Vector3Int end, out Vector2Int hallStart, out Vector2Int hallEnd)
    {

        hallStart = new Vector2Int(0, 0);
        hallEnd = new Vector2Int(0, 0);
        bool isSet = false;


        if (start.y < end.y)
        {
            while (start.y != end.y)
            {
                start.y += 1;

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already
                {
                    if (!isSet) //set the starting position one time
                    {
                        isSet = true;
                        hallStart.x = start.x;
                        hallStart.y = start.y;
                    }
                    hallEnd.x = start.x;
                    hallEnd.y = start.y;

                    mainTilemap.SetTileFlags(start, TileFlags.None);
                    mainTilemap.SetTile(start, gameplayFloorHallwayTile);



                    mainTilemap.SetTileFlags(start + new Vector3Int(-mapOffset, -mapOffset, 0), TileFlags.None);
                    mainTilemap.SetTile(start + new Vector3Int(-mapOffset, -mapOffset, 0), mapFloorAndWallTile);
                }
            }
        }

        else if (start.y > end.y)
        {
            while (start.y != end.y)
            {
                start.y -= 1;

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already
                {
                    if (!isSet) //set the starting position one time
                    {
                        isSet = true;
                        hallEnd.x = start.x; //this makes the "start" hallway lower than the "end"
                        hallEnd.y = start.y;
                    }
                    hallStart.x = start.x;
                    hallStart.y = start.y;

                    mainTilemap.SetTileFlags(start, TileFlags.None);
                    mainTilemap.SetTile(start, gameplayFloorHallwayTile);

                    mainTilemap.SetTileFlags(start + new Vector3Int(-mapOffset, -mapOffset, 0), TileFlags.None);
                    mainTilemap.SetTile(start + new Vector3Int(-mapOffset, -mapOffset, 0), mapFloorAndWallTile);
                }
            }
        }

        //Debug.Log($"Hallway from ({hallStart.x},{hallStart.y}) to ({hallEnd.x},{hallEnd.y})");

        return;
    }

    void DrawRoom(Room room)
    {
        for (int i = room.x; i < (room.x + room.width); ++i)
        {
            for (int j = room.y; j < (room.y + room.height); ++j)
            {
                Vector3Int position = new Vector3Int(i, j, 0);

                mainTilemap.SetTileFlags(position, TileFlags.None);
                mainTilemap.SetTile(position, gameplayFloorTile);

                mainTilemap.SetTileFlags(position + new Vector3Int(-mapOffset, -mapOffset, 0), TileFlags.None);
                mainTilemap.SetTile(position + new Vector3Int(-mapOffset, -mapOffset, 0), mapFloorAndWallTile);

            }
        }
    }

    // A simple BFS algorithm to draw the leaves and spawn the rooms inside them
    void DrawPartitionsAndCreateRooms(Partition root)
    {
        Queue<Partition> queue = new Queue<Partition>();

        queue.Enqueue(root);


        while (queue.Count > 0)
        {

            Partition partition = queue.Peek();

            if (partition.leftChild != null)
            {
                queue.Enqueue(partition.leftChild);
            }

            if (partition.rightChild != null)
            {
                queue.Enqueue(partition.rightChild);
            }

            //Debug.Log($"Partition at: ({partition.x},{partition.y}) has w,h of {partition.width},{partition.height} and isLeaf is {partition.isLeaf}");

            if (partition.isLeaf)
            {
                //_DrawPartitionHelper(partition);

                // Generate random data for the room inside the partition
                int rngWidth = UnityEngine.Random.Range(roomMinimumWidth, Math.Max(partition.width - 2, roomMinimumWidth)); // WE CAN MODIFY THIS WIDTH AND HEIGHT TO GET SOME INTERESTING RESULTS IN ROOM SIZE
                int rngHeight = UnityEngine.Random.Range(roomMinimumHeight, Math.Max(partition.height - 2, roomMinimumHeight)); // FOR EXAMPLE, TRY DIVIDING WIDTH AND HEIGHT BY 2

                // Now we just need to accomodate the width and height
                int rngX = UnityEngine.Random.Range(partition.x, partition.x + partition.width - rngWidth);
                int rngY = UnityEngine.Random.Range(partition.y, partition.y + partition.height - rngHeight);

                int actualMinX = UnityEngine.Mathf.Min(rngWidth, 30);
                int actualMinY = UnityEngine.Mathf.Min(rngHeight, 30);

                int rngOriginX = UnityEngine.Random.Range(rngX + 1, rngX + actualMinX - 1);
                int rngOriginY = UnityEngine.Random.Range(rngY + 1, rngY + actualMinY - 1);

                // Debug.Log($"Creating a room at ({rngX},{rngY}), with width {rngWidth} and height {rngHeight}.");
                // Debug.Log($"The partition was at ({root.x},{root.y}) and had width {root.width} and height {root.height}");


                //Create Game Object for Room
                GameObject roomObj = new GameObject("Room_" + allRooms.Count);
                GameObject roomObjMap = new GameObject("Room_Map_" + allRooms.Count);
                GameObject roomObjGameplay = new GameObject("Room_Gameplay_" + allRooms.Count);

                //set parent objects
                roomObj.transform.SetParent(GameObject.Find("Rooms").transform);
                roomObjMap.transform.SetParent(roomObj.transform); //map is child 0
                roomObjGameplay.transform.SetParent(roomObj.transform); //gameplay is child 1

                //Math to set the locations of the child game object(s)
                float startX = rngX;
                float endX = rngX + rngWidth;
                float startY = rngY;
                float endY = rngY + rngHeight;
                Vector3Int size = new Vector3Int(rngWidth - 2, rngHeight - 2, 0);
                float centerX = (startX + endX) / 2;
                float centerY = (startY + endY) / 2;
                Vector3 center = new Vector3(centerX, centerY, 0);


                //Map Object
                roomObjMap.transform.position = center - new Vector3Int(mapOffset, mapOffset, 0);
                roomObjMap.transform.localScale = size;
                BoxCollider roomCollMap = roomObjMap.AddComponent<BoxCollider>();
                roomCollMap.isTrigger = true;


                //Gameplay Object
                roomObjGameplay.transform.position = center;
                roomObjGameplay.transform.localScale = size;

                BoxCollider2D roomColl = roomObjGameplay.AddComponent<BoxCollider2D>();
                roomColl.isTrigger = true;
                roomObjGameplay.AddComponent<RoomFogController>();

                GameObject fogBoxRoom = Instantiate(fogGameplayPrefab, new Vector3Int(rngX, rngY, 0), Quaternion.identity);
                fogBoxRoom.transform.localScale = new Vector3Int(rngWidth, rngHeight, 0);
                fogBoxRoom.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");
                fogBoxRoom.transform.SetParent(roomObjGameplay.transform);
                fogBoxRoom.name = "Room Fogbox";

                //tags
                roomObjGameplay.tag = "room";
                roomObjMap.tag = "room";

                //Add Room script to parent game object
                Room roomComponent = roomObj.AddComponent<Room>();
                roomComponent.SetupRoom(rngX, rngY, rngWidth, rngHeight, rngOriginX, rngOriginY, allRooms.Count, ref partition);


                //add to list in BSP
                allRooms.Add(roomObj);
                partition.roomId = roomComponent.roomId;
            }
            queue.Dequeue();
        }
    }

    void _DrawPartitionHelper(Partition part)
    {
        UnityEngine.Color color = UnityEngine.Random.ColorHSV(); ;

        for (int i = part.x; i < (part.x + part.width); ++i)
        {
            for (int j = part.y; j < (part.y + part.height); ++j)
            {
                Vector3Int position = new Vector3Int(i, j, 0);

                mainTilemap.SetTile(position, gameplayFloorTile);
                mainTilemap.SetTileFlags(position, TileFlags.None);
                mainTilemap.SetColor(position, color);
            }
        }
    }

    public class Partition
    {
        private int _x, _y, _width, _height, _roomId;
        private bool _isLeaf;

        public Partition leftChild;
        public Partition rightChild;


        public int x
        {
            get { return _x; }
            set { _x = value; }
        }

        public int y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int height
        {
            get { return _height; }
            set { _height = value; }
        }

        public bool isLeaf
        {
            get { return _isLeaf; }
            set { _isLeaf = value; }
        }

        // Room ID will only be used if the partition is a leaf
        public int roomId
        {
            get { return _roomId; }
            set { _roomId = value; }
        }


        public Partition(int x, int y, int width, int height, int minimumRoomSize)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _isLeaf = false;
            _roomId = -1;

            leftChild = null;
            rightChild = null;

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

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


    [Header("Other Stuff")]
    public Tilemap mainTilemap;
    //public Tilemap mapTilemap;

    public List<GameObject> allRooms = new List<GameObject>();
    public List<GameObject> allHallways = new List<GameObject>();

    public float[][] distances;

    public GameObject player;

    public EnemyGeneration Danger_Generator;
    public ObjectGeneration Object_Generator;
    public TrapGeneration Trap_Generator;

    public Partition dungeon;

    public void StartBspGeneration(int dungeonLevel)
    {
        // Get the tilemaps
        mainTilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
        //mapTilemap = GameObject.Find("Map Tilemap").GetComponent<Tilemap>();

        // Get Dungeon Setup
        Danger_Generator = gameObject.GetComponent<EnemyGeneration>();
        Object_Generator = gameObject.GetComponent<ObjectGeneration>();
        Trap_Generator = gameObject.GetComponent<TrapGeneration>();

        // Get the player so we can place them at the correct location
        player = GameObject.Find("Player");

        if (dungeonLevel <= 60)
        {
            mapWidth = 25 + dungeonLevel;
            mapHeight = 25 + dungeonLevel;
        }
        else{
            mapWidth = 85;
            mapHeight = 85;
        }
        // let's make the entire background walls

        // Define the bounds for gameplay wall tiles
        BoundsInt gameplayWallBounds = new BoundsInt(-125, -125, 0, 300, 300, 1);
        TileBase[] gameplayWallTiles = new TileBase[gameplayWallBounds.size.x * gameplayWallBounds.size.y];
        for (int i = 0; i < gameplayWallTiles.Length; i++)
        {
            gameplayWallTiles[i] = gameplayWallTile;
        }
        mainTilemap.SetTilesBlock(gameplayWallBounds, gameplayWallTiles);

        // Define the bounds for map outside tiles
        BoundsInt mapOutsideBounds = new BoundsInt(-125 - mapOffset, -125 - mapOffset, 0, 300, 300, 1);
        TileBase[] mapOutsideTiles = new TileBase[mapOutsideBounds.size.x * mapOutsideBounds.size.y];
        for (int i = 0; i < mapOutsideTiles.Length; i++)
        {
            mapOutsideTiles[i] = mapOutsideTile;
        }
        mainTilemap.SetTilesBlock(mapOutsideBounds, mapOutsideTiles);

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

        //handle hallways
        CreateCorridors(dungeon);
        StartCoroutine(CleanUpAndMergeHallways());

        // now we can place our player at the start of the dungeon.
        // FOR NOW WE ARE PICKING THE FIRST ROOM IN THE LIST, ARBITRARILY

        // player.gameObject.transform.position = new Vector3Int(allRooms[0].GetComponent<Room>().originX, allRooms[0].GetComponent<Room>().originY, 0);

        // Time to set up the dungeon
        // Set Types
        // Safe Room - 20 %

        // Lore Room - 40 %
        // 2 subtypes (Unassigned 25% and Interactive 75%)

        // Danger Room - 40 %
        // 4 subtypes


        // for every room, let's tag it and populate it!
        // First room should be unassigned though so we start at 1
        for (int i = 1; i < allRooms.Count; ++i)
        {

            Room room = allRooms[i].GetComponent<Room>();

            int rand = UnityEngine.Random.Range(0, 100);


            if (rand < 10) // Safe
            {
                room.roomType = Enums.RoomType.Safe;
                room.roomSubType = GetRandomRoomSubType(room.roomType);
            }

            else if (rand < 50) // Lore
            {

                room.roomType = Enums.RoomType.Lore;
                room.roomSubType = GetRandomRoomSubType(room.roomType);

            }
            // Danger
            else
            {

                room.roomType = Enums.RoomType.Danger;
                room.roomSubType = GetRandomRoomSubType(room.roomType);


            }

            if (room.roomSubType == Enums.RoomSubType.TrapEasy || room.roomSubType == Enums.RoomSubType.TrapHard)
            {
                Trap_Generator.GenerateTrap(allRooms[i]);

            }

            else if (room.roomSubType == Enums.RoomSubType.EnemyEasy || room.roomSubType == Enums.RoomSubType.EnemyHard)
            {
                Danger_Generator.GenerateEnemies(allRooms[i]);

            }

        }

        player.transform.position = new Vector3Int(allRooms[0].GetComponent<Room>().originX, allRooms[0].GetComponent<Room>().originY, 0);

        GameObject.Find("Room_Gameplay_0").GetComponentInChildren<SpriteRenderer>().enabled = false;

    }



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


        // //Check if the hallway will touch existing hallways
        // bool isOverlapping = false;
        // GameObject parent = null;

        // //horizontal check
        // if (horizontalStart != Vector2Int.zero)
        // {
        //     //Vector2 h = horizontalStart;
        //     Collider2D col = Physics2D.OverlapArea(horizontalStart, horizontalEnd+Vector2.one ,LayerMask.GetMask("RoomFog"));
        //     //Debug.DrawLine(horizontalStart, horizontalEnd + UnityEngine.Vector2.one, UnityEngine.Color.white);

        // }

        // //vertical check

        // if (verticalStart != Vector2Int.zero)
        // {

        // }


        //Game Object Stuff
        GameObject hallwayObject = new GameObject("Hallway_" + room1.roomId.ToString() + "_" + room2.roomId.ToString());
        hallwayObject.transform.SetParent(GameObject.Find("Hallways").transform);
        allHallways.Add(hallwayObject);


        //attach hallway class and assign
        Hallway hallway = hallwayObject.AddComponent<Hallway>();
        hallway.horizontalStart = horizontalStart;
        hallway.horizontalEnd = horizontalEnd;
        hallway.verticalStart = verticalStart;
        hallway.verticalEnd = verticalEnd;
        hallway.room1Id = room1.roomId;
        hallway.room2Id = room2.roomId;

        //Tag and layer
        hallwayObject.tag = "hallway";
        hallwayObject.layer = LayerMask.NameToLayer("RoomFog");


        //horizontal line object
        if (horizontalStart != Vector2Int.zero)
        {
            GameObject fogBoxHorizontal = Instantiate(fogGameplayPrefab, new Vector3Int(horizontalStart.x, horizontalStart.y, 0), Quaternion.identity);
            fogBoxHorizontal.transform.localScale = new Vector3(horizontalEnd.x - horizontalStart.x + 1, 1, 0);
            //fogBoxHorizontal.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");
            fogBoxHorizontal.transform.SetParent(hallwayObject.transform);
            fogBoxHorizontal.name = "Horizontal Hallway";
            fogBoxHorizontal.tag = "hallway";
        }

        //vertical line object 
        if (verticalStart != Vector2Int.zero)
        {
            GameObject fogBoxVertical = Instantiate(fogGameplayPrefab, new Vector3Int(verticalStart.x, verticalStart.y, 0), Quaternion.identity);
            fogBoxVertical.transform.localScale = new Vector3(1, verticalEnd.y - verticalStart.y + 1, 0);
            //fogBoxVertical.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Fog of War");
            fogBoxVertical.transform.SetParent(hallwayObject.transform);
            fogBoxVertical.name = "Vertical Hallway";
            fogBoxVertical.tag = "hallway";
        }

    }

    public void DrawCooridorX(ref Vector3Int start, ref Vector3Int end, out Vector2Int hallStart, out Vector2Int hallEnd)
    {
        hallStart = new Vector2Int(0, 0);
        hallEnd = new Vector2Int(0, 0);
        bool isSet = false;

        if (start.x < end.x) //from left to right
        {
            while (start.x != end.x)
            {
                start.x += 1;

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already (it will overlap a hallway though)
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
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                    mainTilemap.SetTransformMatrix(start, matrix);

                    //and the map tiles
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

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already (it will overlap a hallway though)
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
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                    mainTilemap.SetTransformMatrix(start, matrix);

                    //and the map tiles
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

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already (it will overlap a hallway though)
                {
                    if (!isSet) //set the starting position one time
                    {
                        isSet = true;
                        hallStart.x = start.x;
                        hallStart.y = start.y;
                    }
                    hallEnd.x = start.x;
                    hallEnd.y = start.y;

                    //gameplay tiles
                    mainTilemap.SetTileFlags(start, TileFlags.None);
                    mainTilemap.SetTile(start, gameplayFloorHallwayTile);
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                    mainTilemap.SetTransformMatrix(start, matrix);

                    //map tiles
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

                if (mainTilemap.GetTile(start) != gameplayFloorTile) // not overlapping a room, tile != floor already (it will overlap a hallway though)
                {
                    if (!isSet) //set the starting position one time
                    {
                        isSet = true;
                        hallEnd.x = start.x; //this makes the "start" hallway lower than the "end"
                        hallEnd.y = start.y;
                    }
                    hallStart.x = start.x;
                    hallStart.y = start.y;

                    //gameplay tiles
                    mainTilemap.SetTileFlags(start, TileFlags.None);
                    mainTilemap.SetTile(start, gameplayFloorHallwayTile);
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                    mainTilemap.SetTransformMatrix(start, matrix);

                    //map tiles
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
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                mainTilemap.SetTransformMatrix(position, matrix);

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
                //roomObjGameplay.AddComponent<RoomFogController>();

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

    private IEnumerator CleanUpAndMergeHallways()
    {

        //make sure to let physics happen first
        yield return new WaitForFixedUpdate();

        for (int i = 0; i < allHallways.Count - 1; i++)
        {
            //check if the hallway overlaps with any of the hallways further down the list, but don't check the combos we already checked
            for (int j = i + 1; j < allHallways.Count; j++)
            {

                bool mergeHallways = false;

                ////for each child (Horizontal or Vertical in hallway i)
                foreach (Transform child in allHallways[i].transform)
                {
                    //same thing but in j
                    foreach (Transform child2 in allHallways[j].transform)
                    {

                        if (child.gameObject.GetComponent<BoxCollider2D>().IsTouching(child2.gameObject.GetComponent<BoxCollider2D>()))
                        {
                            //Debug.Log($"Merge {child.name} and {child2.name}");
                            mergeHallways = true;
                            // Transform parent1 = child.gameObject.transform.parent;
                            // Transform parent2 = child2.gameObject.transform.parent;

                            // //if we havent already added the child to move
                            // if (!childrenToMove.ContainsKey(child2))
                            // {
                            //     childrenToMove.Add(child2, parent1);
                            // }
                        }
                    }
                }

                if (mergeHallways)
                {

                    Transform parent1 = allHallways[i].transform;
                    //Debug.Log($"Merge into {parent1.gameObject.name}");

                    while (allHallways[j].transform.childCount > 0)
                    {
                        //Debug.Log($"There are {allHallways[j].transform.childCount} children to merge");
                        allHallways[j].transform.GetChild(0).SetParent(parent1);
                    }
                }

            }

            // //merge hallways outside the list of children so we dont accidentally skip any
            // foreach (KeyValuePair<Transform, Transform> c in childrenToMove)
            // {
            //     c.Key.SetParent(c.Value);
            // }
        }

    }


    private Enums.RoomSubType GetRandomRoomSubType(Enums.RoomType roomType)
    {

        int rand = 0;

        if (roomType == Enums.RoomType.Danger)
        {

            rand = UnityEngine.Random.Range(200, 204);
        }

        else if (roomType == Enums.RoomType.Safe)
        {

            rand = UnityEngine.Random.Range(300, 303);
        }

        else if (roomType == Enums.RoomType.Lore)
        {
            float spawnRate = UnityEngine.Random.value;

            if(spawnRate < .10f){
                rand = 100; // Treasure
            }
            else if(spawnRate < .5f){
                rand = 101; // Library
            }
            else if(spawnRate < .8f){
                rand = 102; // Armory
            }
            else{
                rand = 103; // Dining
            }
            //rand = UnityEngine.Random.Range(100, 104);
        }

        Enums.RoomSubType subType = (Enums.RoomSubType)rand;

        return subType;
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

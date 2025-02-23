using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSP_Demo : MonoBehaviour
{

    [Header("Dungeon Parameters")]
    public int mapWidth;
    public int mapHeight;
    public int roomMinimumWidth;
    public int roomMinimumHeight;
   
    [Header("Drawing Parameters")]
    public bool drawPartitions;
    public bool drawRooms;
    public bool drawCorridors;

    [Header("Dungeon Tiles")]

    [SerializeField]
    private Tile gameplayFloorTile;

    [SerializeField]
    private Tile gameplayWallTile;

    [SerializeField]
    private Tile gameplayFloorHallwayTile;

    [Header("Other Stuff")]
    public Tilemap mainTilemap;
    //public Tilemap mapTilemap;
    public List<GameObject> allRooms = new List<GameObject>();
    public List<GameObject> allHallways = new List<GameObject>();
    public float[][] distances;

    public Partition dungeon;


    public GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Get the tilemaps
        mainTilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();


        mainCamera = GameObject.Find("Main Camera");
        mainCamera.transform.position = new Vector3(mapWidth / 2.0f, mapHeight / 2.0f, -1f * (mapWidth / 4));
        mainCamera.GetComponent<Camera>().orthographicSize = Math.Max(mapWidth, 50f);
        // let's make the entire background walls

        // Define the bounds for gameplay wall tiles
        BoundsInt gameplayWallBounds = new BoundsInt(-125, -125, 0, 300, 300, 1);
        TileBase[] gameplayWallTiles = new TileBase[gameplayWallBounds.size.x * gameplayWallBounds.size.y];


        gameplayFloorTile.color = Color.white;

        for (int i = 0; i < gameplayWallTiles.Length; i++)
        {
            gameplayWallTiles[i] = gameplayWallTile;
        }

        mainTilemap.SetTilesBlock(gameplayWallBounds, gameplayWallTiles);

        // // Let's create the dungeon's bounds
        dungeon = new Partition(0, 0, mapWidth, mapHeight);

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

            if (drawRooms)
            {
                DrawRoom(room1);
            }


            // calculating distances between all rooms
            for (int j = allRooms[i].GetComponent<Room>().roomId; j < allRooms.Count; j++)
            {

                Room room2 = allRooms[j].GetComponent<Room>();

                Vector3Int room1Origin = new Vector3Int(room1.originX, room1.originY, 0);
                Vector3Int room2Origin = new Vector3Int(room2.originX, room2.originY, 0);
                distances[room1.roomId][j] = Vector3Int.Distance(room1Origin, room2Origin);
            }

        }


        if (drawCorridors)
        {
            //handle hallways
            CreateCorridors(dungeon);
            StartCoroutine(CleanUpAndMergeHallways());
        }

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

            Partition bottom = new Partition(partition.x, partition.y, partition.width, rngOffset);
            Partition top = new Partition(partition.x, partition.y + rngOffset, partition.width, partition.height - rngOffset);

            // Debug.Log($"Force a Top/Bottom split at ({partition.x},{partition.y}) with new parts at ({top.x},{top.y}) adn ({bottom.x},{bottom.y})");

            Tuple<Partition, Partition> splitRooms = Tuple.Create(top, bottom);

            return splitRooms;
        }

        else if (type == Enums.PartitionType.LeftRight)
        {
            int rngOffset = partition.width / 2;

            Partition left = new Partition(partition.x, partition.y, rngOffset, partition.height);
            Partition right = new Partition(partition.x + rngOffset, partition.y, partition.width - rngOffset, partition.height);

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

                Partition bottom = new Partition(partition.x, partition.y, partition.width, rngOffset);
                Partition top = new Partition(partition.x, partition.y + rngOffset, partition.width, partition.height - rngOffset);

                //   Debug.Log($"Random split {partition.x},{partition.y}");

                Tuple<Partition, Partition> splitRooms = Tuple.Create(top, bottom);

                return splitRooms;
            }

            // Vertical Plane
            else
            {
                // randomly decide location on vertical plane
                int rngOffset = UnityEngine.Random.Range(roomMinimumWidth + 2, partition.width - roomMinimumWidth);

                Partition left = new Partition(partition.x, partition.y, rngOffset, partition.height);
                Partition right = new Partition(partition.x + rngOffset, partition.y, partition.width - rngOffset, partition.height);

                //   Debug.Log($"Random split {partition.x},{partition.y}");


                Tuple<Partition, Partition> splitRooms = Tuple.Create(left, right);

                return splitRooms;
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

    void DrawRoom(Room room)
    {
        for (int i = room.x; i < (room.x + room.width); ++i)
        {
            for (int j = room.y; j < (room.y + room.height); ++j)
            {
                Vector3Int position = new Vector3Int(i, j, 0);

                mainTilemap.SetTileFlags(position, TileFlags.None);
                mainTilemap.SetTile(position, gameplayFloorTile);
                mainTilemap.SetColor(position, Color.white);
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                mainTilemap.SetTransformMatrix(position, matrix);

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

            Debug.Log($"Partition at: ({partition.x},{partition.y}) has w,h of {partition.width},{partition.height} and isLeaf is {partition.isLeaf}");

            if (partition.isLeaf)
            {
                _DrawPartitionHelper(partition);

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

                Debug.Log($"Creating a room at ({rngX},{rngY}), with width {rngWidth} and height {rngHeight}.");
                Debug.Log($"The partition was at ({root.x},{root.y}) and had width {root.width} and height {root.height}");


                //Create Game Object for Room
                GameObject roomObj = new GameObject("Room_" + allRooms.Count);
                GameObject roomObjGameplay = new GameObject("Room_Gameplay_" + allRooms.Count);

                //set parent objects
                roomObj.transform.SetParent(GameObject.Find("Rooms").transform);
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


                //Gameplay Object
                roomObjGameplay.transform.position = center;
                roomObjGameplay.transform.localScale = size;

                BoxCollider2D roomColl = roomObjGameplay.AddComponent<BoxCollider2D>();
                roomColl.isTrigger = true;
                //roomObjGameplay.AddComponent<RoomFogController>();

                //tags
                roomObjGameplay.tag = "room";

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
        if (!drawPartitions)
        {
            return;
        }

        float hue = UnityEngine.Random.value;
        float saturation = UnityEngine.Random.Range(0.4f, 1.0f); // Avoid very washed-out colors
        float brightness = UnityEngine.Random.Range(0.1f, 0.5f); // Keep it dark enough for white to stand out

        Color color = Color.HSVToRGB(hue, saturation, brightness);

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

    void DrawCorridor(Room room1, Room room2)
    {

        Vector3Int start = new Vector3Int(room1.originX, room1.originY, 0);
        Vector3Int end = new Vector3Int(room2.originX, room2.originY, 0);

        DrawCooridorX(ref start, ref end, out Vector2Int horizontalStart, out Vector2Int horizontalEnd);
        DrawCooridorY(ref start, ref end, out Vector2Int verticalStart, out Vector2Int verticalEnd);


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
                mainTilemap.SetColor(start, Color.white);
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                mainTilemap.SetTransformMatrix(start, matrix);

            }
        }

        else if (start.x > end.x)//from right to left
        {
            while (start.x != end.x)
            {
                start.x -= 1;


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
                mainTilemap.SetColor(start, Color.white);
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                mainTilemap.SetTransformMatrix(start, matrix);

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
                mainTilemap.SetColor(start, Color.white);
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                mainTilemap.SetTransformMatrix(start, matrix);

            }
        }

        else if (start.y > end.y)
        {
            while (start.y != end.y)
            {
                start.y -= 1;


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
                mainTilemap.SetColor(start, Color.white);
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                mainTilemap.SetTransformMatrix(start, matrix);

            }
        }

        //Debug.Log($"Hallway from ({hallStart.x},{hallStart.y}) to ({hallEnd.x},{hallEnd.y})");

        return;
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


        public Partition(int x, int y, int width, int height)
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


        public static implicit operator Partition(BSPGeneration.Partition v)
        {
            return new Partition(v.x, v.y, v.width, v.height);
        }
    }


}


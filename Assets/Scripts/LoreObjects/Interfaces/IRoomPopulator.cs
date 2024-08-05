using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IRoomPopulator {
   public void PopulateRoom(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects);
}


public class LibraryPopulator : IRoomPopulator {
    public void PopulateRoom(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects)
    {
        Debug.Log("this was a library");
    }
}

public class TreasurePopulator : IRoomPopulator {
    public void PopulateRoom(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects)
    {
        Debug.Log("this was a treasure room");
    }
}

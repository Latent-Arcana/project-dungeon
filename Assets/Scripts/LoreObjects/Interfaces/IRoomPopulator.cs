using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IRoomPopulator
{
    public void PopulateRoom(Tilemap tilemap, Room room);
}


public class LibraryPopulator : IRoomPopulator
{

    // placedObjects[Position] = isOccupied
    Dictionary<Vector3Int, bool> placedObjects = new Dictionary<Vector3Int, bool>();

    private SimplePlacementRule simplePlacement = new SimplePlacementRule();
    private WidePlacementRule widePlacement = new WidePlacementRule();
    private List<GameObject> libraryObjects;

    public LibraryPopulator(List<GameObject> loreObjects)
    {

        libraryObjects = loreObjects;
    }

    public void PopulateRoom(Tilemap tilemap, Room room)
    {
        Debug.Log("populating a library");

        foreach (GameObject libraryObject in libraryObjects)
        {

            LoreObjectBehavior libraryObjectBehavior = libraryObject.GetComponent<LoreObjectBehavior>();


            int attempt = 0;
            int numCreated = 0;

            while (attempt <= 10 && numCreated < libraryObjectBehavior.MaximumNumberAllowed)
            {

                Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

                if (libraryObjectBehavior.ObjectType == Enums.ObjectType.Simple)
                {

                    if (simplePlacement.CanPlaceObject(tilemap, room, placedObjects, position, libraryObjectBehavior.IsWallSpawn))
                    {
                        GameObject.Instantiate(libraryObject, position, Quaternion.identity);
                        placedObjects.Add(position, true);
                        numCreated++;
                    }

                    else{
                        attempt++;
                    }

                }

                else if (libraryObjectBehavior.ObjectType == Enums.ObjectType.Wide)
                {
                    if (widePlacement.CanPlaceObject(tilemap, room, placedObjects, position, libraryObjectBehavior.IsWallSpawn))
                    {
                        GameObject.Instantiate(libraryObject, position, Quaternion.identity);
                        placedObjects.Add(position, true);
                        numCreated++;
                    }
                    else{
                        attempt++;
                    }
                }

                else{
                    attempt++;
                }

            }


        }

    }


}

public class TreasurePopulator : IRoomPopulator
{

    // placedObjects[Position] = isOccupied
    Dictionary<Vector3Int, bool> placedObjects = new Dictionary<Vector3Int, bool>();
    private SimplePlacementRule simplePlacement = new SimplePlacementRule();
    private WidePlacementRule widePlacement = new WidePlacementRule();
    private List<GameObject> treasureRoomObjects;
    public TreasurePopulator(List<GameObject> loreObjects)
    {

        treasureRoomObjects = loreObjects;
    }
    public void PopulateRoom(Tilemap tilemap, Room room)
    {
        Debug.Log("populating a treasure room");
        foreach (GameObject treasureRoomObject in treasureRoomObjects)
        {
            LoreObjectBehavior treasureRoomObjectBehavior = treasureRoomObject.GetComponent<LoreObjectBehavior>();

            int attempt = 0;
            int numCreated = 0;

            while (attempt <= 10 && numCreated < treasureRoomObjectBehavior.MaximumNumberAllowed)
            {

                Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

                if (treasureRoomObjectBehavior.ObjectType == Enums.ObjectType.Simple)
                {
                    if (simplePlacement.CanPlaceObject(tilemap, room, placedObjects, position, treasureRoomObjectBehavior.IsWallSpawn))
                    {
                        GameObject.Instantiate(treasureRoomObject, position, Quaternion.identity);
                        placedObjects.Add(position, true);
                        numCreated++;
                    }
                    
                    else{
                        attempt++;
                    }

                }
                else{
                    attempt++;
                }
            }
        }
    }
}

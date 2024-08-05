using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoreGeneration : MonoBehaviour
{
    Tilemap tilemap;

    [Header("Object Prefabs")]

    [SerializeField]
    public GameObject[] loreObjects;

    // placedObjects[Position] = isOccupied
    Dictionary<Vector3Int, bool> placedObjects = new Dictionary<Vector3Int, bool>();

    public void Awake()
    {
        tilemap = GameObject.Find("Main Tilemap").GetComponent<Tilemap>();
    }

    public void GenerateLore(GameObject roomObj)
    {

        Room room = roomObj.GetComponent<Room>();


        // First let's figure out what kind of Lore room we are

        IRoomPopulator populator = RoomPopulatorFactory.GetPopulator();

        populator.PopulateRoom(tilemap, room, placedObjects);

        // if(subCategory == 2) // Treasure
        // {       
        //     int attempt = 0;
        //     bool objPlaced = false;

        //     while (attempt < 20 && !objPlaced)
        //     {

        //         Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width), UnityEngine.Random.Range(room.y + 1, room.y + room.height), 0);

        //         if(PlaceObject(position, room, loreObjects[0])){
                    
        //             Instantiate(loreObjects[0], position, Quaternion.identity);
                    
        //             placedObjects.Add(position, true);
                    
        //             objPlaced = true;
        //         }

                
        //         ++attempt;
        //     }

        // }

        // else if(subCategory == 3){
        //     int attempt = 0;
        //     bool objPlaced = false;

        //     while (attempt < 20 && !objPlaced)
        //     {

        //         Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

        //         if(PlaceObject(position, room, loreObjects[1])){
                    
        //             Instantiate(loreObjects[1], position, Quaternion.identity);
                    
        //             placedObjects.Add(position, true);
                    
        //             objPlaced = true;
        //         }

                
        //         ++attempt;
        //     }
        




       

    }

    private bool PlaceObject(Vector3Int position, Room room, GameObject obj)
    {
        LoreObjectBehavior loreObjectBehavior = obj.GetComponent<LoreObjectBehavior>();

        IPlaceable loreObjectData = loreObjectBehavior.loreObject as IPlaceable;

        if(loreObjectData == null){
            
            return false;
        }

        bool origin = PointIsAvailable(position);

        // Checking for 1x1 only
        if(loreObjectData.GetWidth() == 1 && loreObjectData.GetWidth() == 1){

            bool up = PointIsAvailable(position + Vector3Int.up);
            bool right = PointIsAvailable(position + Vector3Int.right);
            bool down = PointIsAvailable(position + Vector3Int.down);
            bool left = PointIsAvailable(position + Vector3Int.left);

            if(loreObjectData.IsWallSpawn()){

                up = PointIsWall(position + Vector3Int.up);

                return up && right && down && left && origin;

            }

            else{
                return up && right && down && left && origin;
            }
        }

        // checking for 2x1
        else if(loreObjectData.GetWidth() == 2){
            
            
            bool rightRight = PointIsAvailable(position + Vector3Int.right + Vector3Int.right);
            bool down = PointIsAvailable(position + Vector3Int.down);
            bool rightDown = PointIsAvailable(position + Vector3Int.down + Vector3Int.right);
            bool left = PointIsAvailable(position + Vector3Int.left);          

            if(loreObjectData.IsWallSpawn()){

                bool up = PointIsWall(position + Vector3Int.up);
                bool rightUp = PointIsWall(position + Vector3Int.right + Vector3Int.up);

                return rightRight && down && rightDown && left && up && rightUp && origin;
            }

            else{
                bool up = PointIsAvailable(position + Vector3Int.up);
                bool rightUp = PointIsAvailable(position + Vector3Int.right + Vector3Int.up);

                return rightRight && down && rightDown && left && up && rightUp && origin;
            }
        }

        else{
            return origin;
        }
        
      
    }

    public bool PointIsWall(Vector3Int point){

        return tilemap.GetTile(point).name == "wall-rule";
    }
    public bool PointIsAvailable(Vector3Int point)
    {
        return tilemap.GetTile(point).name == "dungeon-floor" && !placedObjects.ContainsKey(point);

    }

}



// USEFUL CODE FOR LATER
// if(objectBehavior == null){

//     return false;
// }

// IPlaceable objectPlacement = objectBehavior.loreObject as IPlaceable;

// if(objectPlacement == null){

//     return false;
// }
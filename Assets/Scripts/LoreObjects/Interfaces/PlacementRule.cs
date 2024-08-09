using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PlacementRule {
    public abstract bool CanPlaceObject(Tilemap tilemap, Vector3Int position, int width);

    public abstract Vector3Int GetPointInRoom(Room room);

    public bool PointIsOpenFloor(Tilemap tilemap, Vector3Int position){
        return tilemap.GetTile(position).name == "dungeon-floor";
    }

    public bool PointIsWall(Tilemap tilemap, Vector3Int position){
        return tilemap.GetTile(position).name == "wall-rule";
    }
}

public class UpperWallPlacementRule : PlacementRule {

    /// <summary>
    /// Check if we can place the object, in this case on an upper wall.
    /// </summary>
    /// <param name="tilemap"> The game's main tilemap. </param>
    /// <param name="position"> The position we're trying to place something at.</param>
    /// <param name="width"> The width of the object we're placing. </param>
    /// <returns> A boolean telling us if we can place the object here (in this case we're checking to see if it's an upper wall.</returns>
    /// <remarks> We aren't currently handling the case where we have tall objects that go on the upper wall. Will that ever be an issue?</remarks>
    public override bool CanPlaceObject(Tilemap tilemap, Vector3Int position, int width){

        bool upperPointsAreWall = true;

        bool placementPointsAreFloor = true;


        // We have to check every single point above the object to see if it's a wall
        for(int i = 0; i < width; ++i){
            
            Vector3Int upperOffset = new Vector3Int(i, 1, 0);
            Vector3Int widthOffset = new Vector3Int(i, 0, 0);

            if(!PointIsWall(tilemap, position + upperOffset)){
                upperPointsAreWall = false;
            }

            if(!PointIsOpenFloor(tilemap, position + widthOffset)){
                placementPointsAreFloor = false;
            }
        }

        // now we need to check the tiles to our left and right to make sure we aren't blocking a hallway

        Vector3Int left = Vector3Int.left + position;
        Vector3Int leftDown = new Vector3Int(position.x - 1, position.y - 1, 0);


        Vector3Int right = new Vector3Int(position.x + width, position.y, 0);
        Vector3Int rightDown = new Vector3Int(position.x + width, position.y - 1, 0);

        bool blockingHallway = false;
        
        // if the left is open but diagonal down isn't
        if(PointIsOpenFloor(tilemap, left) && !PointIsOpenFloor(tilemap, leftDown)){
            
            blockingHallway = true;
        }

        // if the right is open but diagonal down isn't
        if(PointIsOpenFloor(tilemap, right) && !PointIsOpenFloor(tilemap, rightDown)){
            blockingHallway = true;
        }

        return upperPointsAreWall && placementPointsAreFloor && !blockingHallway;
    }

    public override Vector3Int GetPointInRoom(Room room)
    {
        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

        return position;
    }
}

internal class FloorPlacementRule : PlacementRule
{

    //TODO: FINISH
    public override bool CanPlaceObject(Tilemap tilemap, Vector3Int position, int width){
        

        bool placementPointsAreFloor = true;


        // We have to check every single point on the floor to make sure it's actually floor
        for(int i = 0; i < width; ++i){
            
            Vector3Int widthOffset = new Vector3Int(i, 0, 0);

            if(!PointIsOpenFloor(tilemap, position + widthOffset)){
                placementPointsAreFloor = false;
            }
        }

        return placementPointsAreFloor;

    }

    public override Vector3Int GetPointInRoom(Room room)
    {
        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width - 1), UnityEngine.Random.Range(room.y + 1, room.y + room.height - 1), 0);

        return position;
    }

}
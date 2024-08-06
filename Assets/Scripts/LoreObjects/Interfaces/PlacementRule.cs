using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PlacementRule {
    public abstract bool CanPlaceObject(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position, bool isWallSpawn);
    public abstract void PlaceObject(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position);

    public bool PointIsOpenFloor(Tilemap tilemap, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position){
        return tilemap.GetTile(position).name == "dungeon-floor" && !placedObjects.ContainsKey(position);
    }

    public bool PointIsWall(Tilemap tilemap, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position){
        return tilemap.GetTile(position).name == "wall-rule";
    }
}


// 1x1 objects
public class SimplePlacementRule : PlacementRule
{
    public override bool CanPlaceObject(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position, bool isWallSpawn)
    {
        bool _base = PointIsOpenFloor(tilemap, placedObjects, position);

        bool _right = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.right);
        bool _left = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.left);
        bool _down = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.down);

        bool _up = false;

        if(isWallSpawn){
            _up = PointIsWall(tilemap, placedObjects, position + Vector3Int.up);
        }
        else{
            _up = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.up);
        }

        return _base && _right && _left && _down && _up;

    }

    public override void PlaceObject(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position)
    {
        throw new System.NotImplementedException();
    }
}

// 2x1 objects
public class WidePlacementRule : PlacementRule {

    public override bool CanPlaceObject(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position, bool isWallSpawn)
    {
        bool _base_left = PointIsOpenFloor(tilemap, placedObjects, position);
        bool _base_right = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.right);

        bool _right = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.right + Vector3Int.right); // 2 to the right instead of 1
        bool _left = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.left);
        bool _down = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.down);
        bool _down_diag = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.down + Vector3Int.right); // 1 over and 1 down

        bool _up = false;
        bool _up_diag = false;

        if(isWallSpawn){
            _up = PointIsWall(tilemap, placedObjects, position + Vector3Int.up);
            _up_diag = PointIsWall(tilemap, placedObjects, position + Vector3Int.up + Vector3Int.right);
        }
        else{
            _up = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.up);
            _up_diag = PointIsOpenFloor(tilemap, placedObjects, position + Vector3Int.up + Vector3Int.right);
        }

        return _base_left && _base_right && _right && _left && _down && _up && _down_diag && _up_diag;
    }

    public override void PlaceObject(Tilemap tilemap, Room room, Dictionary<Vector3Int, bool> placedObjects, Vector3Int position)
    {
        throw new System.NotImplementedException();
    }
}
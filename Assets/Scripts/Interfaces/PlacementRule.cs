using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PlacementRule
{
    public abstract Vector3Int CanPlaceObject(Tilemap tilemap, Room room, int width, int height);

    public abstract Vector3Int GetPointInRoom(Room room, int objectWidth, int objectHeight);

    public bool PointIsOpenFloor(Tilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position).name == "dungeon-floor" || tilemap.GetTile(position).name == "dungeon-floor-hallway";
    }

    public bool PointIsWall(Tilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position).name == "wall-rule";
    }
}

public class UpperWallPlacementRule : PlacementRule
{

    /// <summary>
    /// Check if we can place the object, in this case on an upper wall.
    /// </summary>
    /// <param name="tilemap"> The game's main tilemap. </param>
    /// <param name="position"> The position we're trying to place something at.</param>
    /// <param name="width"> The width of the object we're placing. </param>
    /// <returns> A boolean telling us if we can place the object here (in this case we're checking to see if it's an upper wall.</returns>
    /// <remarks> We aren't currently handling the case where we have tall objects that go on the upper wall. Will that ever be an issue?</remarks>
    public override Vector3Int CanPlaceObject(Tilemap tilemap, Room room, int width, int height)
    {

        Vector3Int position = GetPointInRoom(room, width, height);

        bool upperPointsAreWall = true;

        bool placementPointsAreFloor = true;

        for (int i = 0; i < width; ++i)
        {

            // We have to check every single point above the object to see if it's a wall
            Vector3Int upperOffset = new Vector3Int(i, height, 0);
            if (!PointIsWall(tilemap, position + upperOffset))
            {
                upperPointsAreWall = false;
            }

            // Then we just check every point of the object to make sure it's all on the floor
            for (int j = 0; j < height; ++j)
            {

                Vector3Int widthOffset = new Vector3Int(i, 0, 0);
                Vector3Int heightOffest = new Vector3Int(0, j, 0);

                if (!PointIsOpenFloor(tilemap, position + widthOffset + heightOffest))
                {
                    placementPointsAreFloor = false;
                }

            }

        }

        // now we need to check the tiles to our left and right to make sure we aren't blocking a hallway



        bool noBlockedHallway = true;


        for (int i = 0; i < height; ++i)
        {
            Vector3Int left = new Vector3Int(position.x - 1, position.y + i, 0);
            Vector3Int leftDown = new Vector3Int(position.x - 1, position.y - 1, 0);


            Vector3Int right = new Vector3Int(position.x + width, position.y + i, 0);
            Vector3Int rightDown = new Vector3Int(position.x + width, position.y - 1, 0);


            // If our leftside point is open but either up or down aren't available to move through, we've blocked a hallway
            if (PointIsOpenFloor(tilemap, left) && PointIsWall(tilemap, leftDown))
            {
                noBlockedHallway = false;
            }

            // if our rightside point is open but either up or down aren't available to move through, we've blocked a hallway
            if (PointIsOpenFloor(tilemap, right) && PointIsWall(tilemap, rightDown))
            {
                noBlockedHallway = false;
            }

        }


        if (upperPointsAreWall && placementPointsAreFloor && noBlockedHallway)
        {
            return position;
        }

        else
        {
            return Vector3Int.zero;
        }
    }

    public override Vector3Int GetPointInRoom(Room room, int objectWidth, int objectHeight)
    {
        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y + (room.height / 2), room.y + room.height), 0);

        return position;
    }
}

public class FloorPlacementRule : PlacementRule
{

    public override Vector3Int CanPlaceObject(Tilemap tilemap, Room room, int width, int height)
    {

        Vector3Int position = GetPointInRoom(room, width, height);


        bool placementPointsAreFloor = true;


        // We have to check every single point on the floor to make sure it's actually floor
        for (int i = 0; i < width; ++i)
        {

            Vector3Int widthOffset = new Vector3Int(i, 0, 0);

            for (int j = 0; j < height; ++j)
            {
                Vector3Int heightOffset = new Vector3Int(0, j, 0);

                if (!PointIsOpenFloor(tilemap, position + widthOffset + heightOffset))
                {
                    placementPointsAreFloor = false;
                }
            }

        }

        if (placementPointsAreFloor)
        {
            return position;
        }

        else
        {
            return Vector3Int.zero;
        }

    }

    public override Vector3Int GetPointInRoom(Room room, int objectWidth, int objectHeight)
    {
        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x + 1, room.x + room.width - objectWidth), UnityEngine.Random.Range(room.y + 1, room.y + room.height - objectHeight), 0);

        return position;
    }

}

public class SideWallPlacementRule : PlacementRule
{
    public Tuple<bool, bool> LeftOrRightWall(Tilemap tilemap, Vector3Int position, int width, int height)
    {

        // Let's find out what side wall we are, and ensure that that whole side is wall

        bool leftSideIsWall = true;
        bool rightSideIsWall = true;

        for (int i = 0; i < height; ++i)
        {

            Vector3Int heightOffset = new Vector3Int(0, i, 0);
            Vector3Int right = new Vector3Int(position.x + width, position.y, 0);
            Vector3Int left = new Vector3Int(position.x - 1, position.y, 0);

            if (!PointIsWall(tilemap, left + heightOffset))
            {
                leftSideIsWall = false;
            }

            if (!PointIsWall(tilemap, right + heightOffset))
            {
                rightSideIsWall = false;
            }

        }

        return new Tuple<bool, bool>(leftSideIsWall, rightSideIsWall);
    }

    public override Vector3Int CanPlaceObject(Tilemap tilemap, Room room, int width, int height)
    {
        Vector3Int position = GetPointInRoom(room, width, height);

        bool placementPointsAreFloor = true;

        // We have to check every single point on the floor to make sure it's actually floor
        for (int i = 0; i < width; ++i)
        {
            Vector3Int widthOffset = new Vector3Int(i, 0, 0);

            for (int j = 0; j < height; ++j)
            {
                Vector3Int heightOffset = new Vector3Int(0, j, 0);

                if (!PointIsOpenFloor(tilemap, position + widthOffset + heightOffset))
                {
                    placementPointsAreFloor = false;
                }
            }


        }

        // Let's find out what side wall we are, and ensure that that whole side is wall

        Tuple<bool, bool> wallChoice = LeftOrRightWall(tilemap, position, width, height);
        bool leftSideIsWall = wallChoice.Item1;
        bool rightSideIsWall = wallChoice.Item2;

        // Now we just need to check above and below to make sure we aren't blocking anything
        bool noBlockedHallway = true;

        for (int i = 0; i < width; ++i)
        {

            Vector3Int abovePointOffset = new Vector3Int(position.x + i, position.y + height, 0);
            Vector3Int belowPointOffset = new Vector3Int(position.x + i, position.y - 1, 0);


            // If the above point is a floor and both of its adjacent points are wall, we are blocked
            if (PointIsOpenFloor(tilemap, abovePointOffset) && PointIsWall(tilemap, abovePointOffset + Vector3Int.right) && PointIsWall(tilemap, abovePointOffset + Vector3Int.left))
            {

                noBlockedHallway = false;
            }

            // If the below point is a floor and both of its adjacent points are wall, we are blocked
            if (PointIsOpenFloor(tilemap, belowPointOffset) && PointIsWall(tilemap, belowPointOffset + Vector3Int.right) && PointIsWall(tilemap, belowPointOffset + Vector3Int.left))
            {

                noBlockedHallway = false;
            }

        }


        if (placementPointsAreFloor && (leftSideIsWall || rightSideIsWall) && noBlockedHallway)
        {
            return position;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    public override Vector3Int GetPointInRoom(Room room, int objectWidth, int objectHeight)
    {
        Vector3Int position = new Vector3Int(UnityEngine.Random.Range(room.x, room.x + room.width), UnityEngine.Random.Range(room.y, room.y + room.height), 0);

        return position;
    }
}
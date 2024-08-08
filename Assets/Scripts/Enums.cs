using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
      public enum RoomType
    {
        Lore,
        Safe,
        Danger,
        Unassigned
    }

    public enum PartitionType
    {
        TopBottom,
        LeftRight,
        Random
    }

    public enum LoreRoomSubType {
        
        Library = 0,
        Treasure = 1
    }

    public enum PlacementType {
        Floor,
        UpperWall,
        SideWall
    }

}

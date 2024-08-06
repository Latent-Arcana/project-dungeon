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
        
        Default = 1,
        Treasure = 2,
        Library = 3,
        Dining = 4
    }

    public enum ObjectType {
        Simple,
        Wide,
        Tall,
        Large
    }

}

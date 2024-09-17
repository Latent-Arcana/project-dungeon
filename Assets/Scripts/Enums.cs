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

    public enum RoomSubType
    {
        Undefined = 0,
        Treasure = 1,
        Library = 2,
        None = 3,
        Armory = 4,
        Dining = 5,
        Trap = 6,
        Enemy = 7
    }

    public enum ObjectType
    {
        Undefined = 0,
        Bookshelf,
        Candle,
        Debris,
        Chest,
        Bed,
        Corpse,
        Chair,
        Table,
        ArmorStand
    }

    public enum PlacementType
    {
        Floor,
        UpperWall,
        SideWall
    }


    public enum ItemType
    {
        General,
        Armor,
        Weapon,
        Consumable,
        Special,
        Book
    }

}

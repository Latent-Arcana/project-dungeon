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
        Unassigned = 0,
        
        Treasure = 100,
        Library = 101,
        Armory = 102,
        Dining = 103,
       
        TrapEasy = 200,
        TrapHard = 201,
        EnemyEasy = 202,
        EnemyHard = 203,
        
        
        Bedroom = 300,
        Hospital = 301,
        Shrine = 302
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
        TableAndChair,
        ArmorStand,
        Spikes, 
        Crates,
        Furnace,
        Mushroom,
        Pile,
        Shrine
    }

    public enum PlacementType
    {
        Floor,
        UpperWall,
        SideWall
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Epic
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

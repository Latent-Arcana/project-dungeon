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
        Bookshelf = 1,
        Candle = 2,
        Debris = 3,
        Chest = 4,
        Bed = 5,
        Corpse = 6,
        Chair = 7,
        Table = 8,
        TableAndChair = 9,
        ArmorStand = 10,
        Spikes = 11,
        Crates = 12,
        Furnace = 13,
        Mushroom = 14,
        Pile = 15,
        Shrine = 16,
        BedHospital = 17,
        Desk = 18,
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

    public enum ShrineType
    {
        Strength = 1,
        Speed = 2,
        Agility = 3
    }

}

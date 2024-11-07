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
        Treasure = 1,
        Library = 2,
        Armory = 3,
        Dining = 4,
        Trap = 5,
        Enemy = 6,
        Safe = 7
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
        BrokenChair,
        Furnace,
        Mushroom,
        Pile
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

using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Shrine")]
public class SubTypeShrine : RoomSubType
{
    /// <summary>
    /// Objects that are required to be in the room will be spawned first.
    /// Listing an object more than once will generate multiple objects.
    /// Ignores MaxAllowed list, but objects generated from here are still included in it's check.
    /// </summary>
    public override List<Enums.ObjectType> RequiredObjects => new()
    {
        Enums.ObjectType.Shrine,
        Enums.ObjectType.Chest
    };

    /// <summary>
    /// Objects that are allowed to be spawned in the room
    /// Any object entered here must also be in the MaxAllowed dictionary below
    /// </summary>
    public override List<Enums.ObjectType> DecorObjects => new()
     {
          Enums.ObjectType.Candle,
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Debris
     };

    /// <summary>
    /// Lists the maximum allowed number of objects of ObjectType that can spawn in the room.
    /// This list is checked before spawning in a new decor object.
    /// </summary>
    public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
     {
        {Enums.ObjectType.Shrine,1},
        {Enums.ObjectType.Candle,4},
        {Enums.ObjectType.Bookshelf,2},
        {Enums.ObjectType.Debris,2},
        {Enums.ObjectType.Chest,1}
    };


}
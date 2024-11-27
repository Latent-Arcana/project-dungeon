using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Trap Hard")]
public class SubTypeTrapHard : RoomSubType
{
    /// <summary>
    /// Objects that are required to be in the room will be spawned first.
    /// Listing an object more than once will generate multiple objects.
    /// </summary>
    public override List<Enums.ObjectType> RequiredObjects => new()
    {
        Enums.ObjectType.Mushroom
    };

    /// <summary>
    /// Objects that are allowed to be spawned in the room
    /// Any object entered here must also be in the MaxAllowed dictionary below
    /// </summary>
    public override List<Enums.ObjectType> DecorObjects => new()
     {
        Enums.ObjectType.Bookshelf,
        Enums.ObjectType.Debris,
        Enums.ObjectType.Crates,
        Enums.ObjectType.Mushroom,
        Enums.ObjectType.Chest,
        Enums.ObjectType.Spikes,
     };

    /// <summary>
    /// Lists the maximum allowed number of objects of ObjectType that can spawn in the room.
    /// This list is checked before spawning in a new object.
    /// </summary>
    public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
     {
        {Enums.ObjectType.Bookshelf,2},
        {Enums.ObjectType.Debris,2},
        {Enums.ObjectType.Crates,2},
        {Enums.ObjectType.Mushroom,2},
        {Enums.ObjectType.Chest,1},
        {Enums.ObjectType.Spikes,2}
    };

}
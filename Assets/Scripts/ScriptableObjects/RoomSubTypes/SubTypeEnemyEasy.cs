using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Enemy Easy")]
public class SubTypeEnemyEasy : RoomSubType
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
          Enums.ObjectType.Candle,
          Enums.ObjectType.ArmorStand,
          Enums.ObjectType.Debris,
          Enums.ObjectType.Mushroom,
          Enums.ObjectType.Spikes
     };

    /// <summary>
    /// Lists the maximum allowed number of objects of ObjectType that can spawn in the room.
    /// This list is checked before spawning in a new object.
    /// </summary>
    public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
     {
        {Enums.ObjectType.Candle,2},
        {Enums.ObjectType.ArmorStand,1},
        {Enums.ObjectType.Debris,2},
        {Enums.ObjectType.Mushroom,4},
        {Enums.ObjectType.Spikes,2}

    };


}
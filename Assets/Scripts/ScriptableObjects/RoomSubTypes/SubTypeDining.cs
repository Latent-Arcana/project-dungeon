using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Dining")]
public class SubTypeDining : RoomSubType
{
    /// <summary>
    /// Objects that are required to be in the room will be spawned first.
    /// Listing an object more than once will generate multiple objects.
    /// </summary>
    public override List<Enums.ObjectType> RequiredObjects => new()
    {
        Enums.ObjectType.TableAndChair,
        Enums.ObjectType.Candle
    };

    /// <summary>
    /// Objects that are allowed to be spawned in the room
    /// Any object entered here must also be in the MaxAllowed dictionary below
    /// </summary>
    public override List<Enums.ObjectType> DecorObjects => new()
     {
          Enums.ObjectType.Candle,
          Enums.ObjectType.Chair,
          Enums.ObjectType.Table,
          Enums.ObjectType.Crates,
          Enums.ObjectType.TableAndChair,
          Enums.ObjectType.Furnace,
          Enums.ObjectType.Mushroom
     };

    /// <summary>
    /// Lists the maximum allowed number of objects of ObjectType that can spawn in the room.
    /// This list is checked before spawning in a new object.
    /// </summary>
    public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
     {
        {Enums.ObjectType.Candle,4},
        {Enums.ObjectType.Chair,2},
        {Enums.ObjectType.Table,2},
        {Enums.ObjectType.Crates,2},
        {Enums.ObjectType.TableAndChair,2},
        {Enums.ObjectType.Furnace,1},
        {Enums.ObjectType.Mushroom,2}
    };


}
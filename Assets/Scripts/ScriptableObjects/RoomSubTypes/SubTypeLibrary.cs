using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Library")]
public class SubTypeLibrary : RoomSubType
{
    /// <summary>
    /// Objects that are required to be in the room will be spawned first.
    /// Listing an object more than once will generate multiple objects.
    /// </summary>
    public override List<Enums.ObjectType> RequiredObjects => new()
     {
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Desk,
     };

    /// <summary>
    /// Objects that are allowed to be spawned in the room
    /// Any object entered here must also be in the MaxAllowed dictionary below
    /// </summary>
    public override List<Enums.ObjectType> DecorObjects => new()
     {
          Enums.ObjectType.Candle,
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Debris,
          Enums.ObjectType.Mushroom,
          Enums.ObjectType.TableAndChair,
          Enums.ObjectType.Chair,
          Enums.ObjectType.Desk,
     };

    /// <summary>
    /// Lists the maximum allowed number of objects of ObjectType that can spawn in the room.
    /// This list is checked before spawning in a new object.
    /// </summary>
    public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
    {
        {Enums.ObjectType.Bookshelf,5},
        {Enums.ObjectType.Debris,2},
        {Enums.ObjectType.Candle,3},
        {Enums.ObjectType.Mushroom,2},
        {Enums.ObjectType.TableAndChair,1},
        {Enums.ObjectType.Chair,1},
        {Enums.ObjectType.Desk,2}
    };

}
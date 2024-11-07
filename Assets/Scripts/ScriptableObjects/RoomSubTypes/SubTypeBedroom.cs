using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Bedroom")]
public class SubTypeBedroom : RoomSubType
{
     //     [SerializeField]
     //     //public Dictionary<Enums.ObjectType, int> maxAllowed;

     //     public new Dictionary<Enums.ObjectType, int> maxAllowed = new()
     // {
     //     [Enums.ObjectType.Bookshelf] = 3
     // };

     //     public new Enums.ObjectType[] RequiredObjects;

     //     public Enums.ObjectType[] DecorObjects;

     public override List<Enums.ObjectType> RequiredObjects => new()
     {
          Enums.ObjectType.Bed
     };


     public override List<Enums.ObjectType> DecorObjects => new()
     {
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Candle,
          Enums.ObjectType.Chest,
          Enums.ObjectType.Chair,
          Enums.ObjectType.Table,
          Enums.ObjectType.Crates,
          Enums.ObjectType.Debris
     };

     public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
     {
        {Enums.ObjectType.Bookshelf,2},
        {Enums.ObjectType.Candle,2},
        {Enums.ObjectType.Chest,1},
        {Enums.ObjectType.Chair,1},
        {Enums.ObjectType.Table,1},
        {Enums.ObjectType.Crates,1},
        {Enums.ObjectType.Debris,1}

    };


}
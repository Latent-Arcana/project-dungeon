using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(menuName = "Room Subtype/Library")]
public class SubTypeLibrary : RoomSubType
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
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Bookshelf
     };


    public override List<Enums.ObjectType> DecorObjects => new() 
     {
          Enums.ObjectType.Candle,
          Enums.ObjectType.Bookshelf,
          Enums.ObjectType.Debris,
          Enums.ObjectType.Mushroom,
          Enums.ObjectType.TableAndChair,
          Enums.ObjectType.Chair
     };



    public override Dictionary<Enums.ObjectType, int> MaxAllowed => new()
    {
        {Enums.ObjectType.Bookshelf,5},
        {Enums.ObjectType.Debris,2},
        {Enums.ObjectType.Candle,3},
        {Enums.ObjectType.Mushroom,2},
        {Enums.ObjectType.TableAndChair,1},
        {Enums.ObjectType.Chair,1}

    };

}
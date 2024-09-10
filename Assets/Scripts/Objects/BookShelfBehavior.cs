using UnityEngine;
using System.Collections.Generic;

public class BookShelfBehavior : ObjectBehavior
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> {Enums.RoomType.Lore};

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> {Enums.RoomSubType.Library};

    public override Enums.ObjectType ObjectType => Enums.ObjectType.Bookshelf;

    public override Enums.PlacementType PlacementType => Enums.PlacementType.UpperWall;

    public override int Width => 1;

    public override int Height => 1;

    public override string Look()
    {
        return "NOT IMPLEMENTED";
    }
}

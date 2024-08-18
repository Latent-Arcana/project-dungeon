using UnityEngine;
using System.Collections.Generic;

public class WideBookShelfBehavior : ObjectBehavior, IOpenable
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> {Enums.RoomType.Lore};

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> {Enums.RoomSubType.Library};

    public override Enums.PlacementType PlacementType => Enums.PlacementType.UpperWall;

    public override Enums.ObjectType ObjectType => Enums.ObjectType.Bookshelf;

    public override int Width => 2;

    public override int Height => 1;

    public override string Look()
    {
        return "NOT IMPLEMENTED";
    }

    public Item[] GetItems()
    {
        throw new System.NotImplementedException();
    }

    public void Open()
    {
        throw new System.NotImplementedException();
    }
}

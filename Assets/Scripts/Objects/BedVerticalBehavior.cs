using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedVerticalBehavior : ObjectBehavior
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> {Enums.RoomType.Safe};

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> {Enums.RoomSubType.None};

    public override Enums.PlacementType PlacementType => Enums.PlacementType.SideWall;

    public override int Width => 2;

    public override int Height => 1;

    public override int MaximumNumberAllowed => 1;

    public override GameObject Interact()
    {
        return null;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class DebrisSmallBehavior : ObjectBehavior
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> {Enums.RoomType.Lore, Enums.RoomType.Danger, Enums.RoomType.Unassigned, Enums.RoomType.Safe};

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> {Enums.RoomSubType.Library, Enums.RoomSubType.Treasure};
    public override Enums.PlacementType PlacementType => Enums.PlacementType.Floor;

    public override int Width => 1;

    public override int Height => 1;
    public override int MaximumNumberAllowed => 2;


    public override GameObject Interact()
    {
        return null;
    }
}

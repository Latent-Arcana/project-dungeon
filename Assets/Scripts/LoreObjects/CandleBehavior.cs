using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CandleBehavior : ObjectBehavior
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> {Enums.RoomType.Lore};

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> {Enums.RoomSubType.Library, Enums.RoomSubType.Treasure};

    public override Enums.PlacementType PlacementType => Enums.PlacementType.Floor;

    public override int Width => 1;

    public override int Height => 1;

    public override int MaximumNumberAllowed => 5;

    public override GameObject Interact()
    {
        return null;
    }
}
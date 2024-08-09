using UnityEngine;

public class CandleBehavior : LoreObjectBehavior
{
    public override Enums.LoreRoomSubType SubType => Enums.LoreRoomSubType.Library;

    public override Enums.PlacementType PlacementType => Enums.PlacementType.Floor;

    public override int Width => 1;

    public override int Height => 1;

    public override int MaximumNumberAllowed => 5;

    public override GameObject Interact()
    {
        return null;
    }
}
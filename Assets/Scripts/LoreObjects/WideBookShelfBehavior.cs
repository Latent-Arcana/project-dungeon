using UnityEngine;

public class WideBookShelfBehavior : LoreObjectBehavior
{
    public override Enums.LoreRoomSubType SubType => Enums.LoreRoomSubType.Library;

    public override Enums.PlacementType PlacementType => Enums.PlacementType.UpperWall;

    public override int Width => 2;

    public override int Height => 1;
    public override int MaximumNumberAllowed => 4;


    public override GameObject Interact()
    {
        return null;
    }
}

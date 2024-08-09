using UnityEngine;

public class TallBookShelfBehavior : LoreObjectBehavior
{
    public override Enums.LoreRoomSubType SubType => Enums.LoreRoomSubType.Library;

    public override Enums.PlacementType PlacementType => Enums.PlacementType.UpperWall;

    public override int Width => 1;

    public override int Height => 2;
    public override int MaximumNumberAllowed => 4;


    public override GameObject Interact()
    {
        return null;
    }
}

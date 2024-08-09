using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureChestBehavior : LoreObjectBehavior
{
    public override Enums.LoreRoomSubType SubType => Enums.LoreRoomSubType.Treasure;

    public override Enums.PlacementType PlacementType => Enums.PlacementType.Floor;

    public override int Width => 1;

    public override int Height => 1;

    public override int MaximumNumberAllowed => 1;

    public override GameObject Interact()
    {
        return null;
    }
}

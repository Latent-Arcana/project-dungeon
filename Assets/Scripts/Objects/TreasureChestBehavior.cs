using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureChestBehavior : ObjectBehavior, IOpenable
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> { Enums.RoomType.Lore };

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> { Enums.RoomSubType.Treasure };

    public override Enums.ObjectType ObjectType => Enums.ObjectType.Chest;

    public override Enums.PlacementType PlacementType => Enums.PlacementType.Floor;

    public override int Width => 1;

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

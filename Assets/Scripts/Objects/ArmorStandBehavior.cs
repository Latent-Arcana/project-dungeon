using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArmorStandBehavior : ObjectBehavior
{
    public override List<Enums.RoomType> RoomTypes => new List<Enums.RoomType> {Enums.RoomType.Danger, Enums.RoomType.Lore};

    public override List<Enums.RoomSubType> RoomSubTypes => new List<Enums.RoomSubType> {Enums.RoomSubType.Armory};

    public override Enums.ObjectType ObjectType => Enums.ObjectType.ArmorStand;


    public override Enums.PlacementType PlacementType => Enums.PlacementType.Floor;

    public override int Width => 1;

    public override int Height => 1;
    public override string Look()
    {
        return "No time to sit, keep pressing onwards.";
    }
}
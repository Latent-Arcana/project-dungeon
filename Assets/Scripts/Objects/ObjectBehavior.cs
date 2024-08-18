using System.Collections.Generic;
using UnityEngine;
using static Enums;

public abstract class ObjectBehavior : MonoBehaviour, IPlaceable, ILookable
{
    public abstract List<RoomType> RoomTypes
    {
        get;
    }

    public abstract List<Enums.RoomSubType> RoomSubTypes
    {
        get;
    }

    public abstract ObjectType ObjectType
    {
        get;
    }

    public abstract PlacementType PlacementType
    {
        get;
    }


    public abstract int Width
    {
        get;
    }

    public abstract int Height
    {
        get;
    }

    public abstract string Look();
}

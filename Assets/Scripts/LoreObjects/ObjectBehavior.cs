using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Enums;


public abstract class ObjectBehavior : MonoBehaviour, IInteractable, IPlaceable
{
    public abstract List<RoomType> RoomTypes
    {
        get;
    }

    public abstract List<Enums.RoomSubType> RoomSubTypes
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

    public abstract int MaximumNumberAllowed
    {
        get;
    }

    public abstract GameObject Interact();
}

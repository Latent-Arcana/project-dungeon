using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Enums;


public abstract class LoreObjectBehavior : MonoBehaviour, IInteractable, IPlaceable
{
    public abstract LoreRoomSubType SubType{
        get;
    }

    public abstract ObjectType ObjectType{
        get;
    }

    public abstract int Width{
        get;
    }

    public abstract int Height{
        get;
    }

    public abstract bool IsWallSpawn{
        get;
    }

    public abstract int MaximumNumberAllowed{
        get;
    }

    public abstract GameObject Interact();
}

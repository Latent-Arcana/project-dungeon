using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class ObjectBehavior : MonoBehaviour, ILookable
{
    public List<RoomType> RoomTypes;

    public List<Enums.RoomSubType> RoomSubTypes;

    public ObjectType ObjectType;

    public PlacementType PlacementType;

    public int Width;

    public int Height;


    public string[] lookStrings;

    public string Look()
    {
        //TODO: make random
        return lookStrings[0];
    }


}

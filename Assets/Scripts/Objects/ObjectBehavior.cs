using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class ObjectBehavior : MonoBehaviour, ILookable
{

    public ObjectType ObjectType;

    public PlacementType PlacementType;

    public int Width;

    public int Height;

    public Sprite flippedSprite;

    public bool flipped;

    public string[] lookStrings;

    public string Look()
    {
        return lookStrings[0];
    }


}

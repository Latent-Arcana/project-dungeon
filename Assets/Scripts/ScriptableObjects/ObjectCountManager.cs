using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Object Count Manager")]
public class ObjectCountManager : ScriptableObject
{
    // Maximum allowed
    public int maxBookShelfCount = 5;
    public int maxCandleCount = 6;
    public int maxDebrisCount = 6;
    public int maxChestCount = 1;
    public int maxBedCount = 1;


    public int GetCountAllowedByObjectType(Enums.ObjectType objectType)
    {

        if (objectType == Enums.ObjectType.Bed) { return maxBedCount; }
        else if (objectType == Enums.ObjectType.Bookshelf) { return maxBookShelfCount; }
        else if (objectType == Enums.ObjectType.Candle) { return maxCandleCount; }
        else if (objectType == Enums.ObjectType.Chest) { return maxChestCount; }
        else if (objectType == Enums.ObjectType.Debris) { return maxDebrisCount; }
        else
        {
            return 1000; // return a huge number so we just run out of attempts instead of running out of maxCount
        }

    }
}
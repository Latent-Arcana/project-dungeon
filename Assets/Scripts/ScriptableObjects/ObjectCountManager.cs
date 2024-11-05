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
    public int maxCandleCount = 4;
    public int maxDebrisCount = 4;
    public int maxChestCount = 1;
    public int maxBedCount = 1;
    public int maxArmorStandCount = 2;
    public int maxChairCount = 3;
    public int maxTableCount = 2;
    public int maxSpikesCount = 3;


    public int GetCountAllowedByObjectType(Enums.ObjectType objectType)
    {

        if (objectType == Enums.ObjectType.Bed) { return maxBedCount; }
        else if (objectType == Enums.ObjectType.Bookshelf) { return maxBookShelfCount; }
        else if (objectType == Enums.ObjectType.Candle) { return maxCandleCount; }
        else if (objectType == Enums.ObjectType.Chest) { return maxChestCount; }
        else if (objectType == Enums.ObjectType.Debris) { return maxDebrisCount; }
        else if (objectType == Enums.ObjectType.ArmorStand) { return maxArmorStandCount; }
        else if (objectType == Enums.ObjectType.Chair) { return maxChairCount; }
        else if (objectType == Enums.ObjectType.Table) { return maxTableCount; }
        else if (objectType == Enums.ObjectType.Spikes) { return maxSpikesCount; }
        else
        {
            return 1000; // return a huge number so we just run out of attempts instead of running out of maxCount
        }

    }
}
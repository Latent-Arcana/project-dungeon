using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Treasure Chest")]
public class TreasureChestScriptableObject : ScriptableObject, IPlaceable
{
    public Enums.LoreRoomSubType loreRoomSubType = Enums.LoreRoomSubType.Treasure;

    public int width = 1;

    public int height = 1;

    public bool isWallSpawn = false;

    public int GetHeight()
    {
        return height;
    }

    public Enums.LoreRoomSubType GetSubType()
    {
        return loreRoomSubType;
    }

    public int GetWidth()
    {
        return width;
    }

    public bool IsWallSpawn()
    {
        return isWallSpawn;
    }
}

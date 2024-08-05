using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Wide Book Shelf")]
public class WideBookShelfScriptableObject : ScriptableObject, IPlaceable
{
    public Enums.LoreRoomSubType loreRoomSubType = Enums.LoreRoomSubType.Library;

    public int width = 2;

    public int height = 1;

    public bool isWallSpawn = true;

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlaceable
{
   public Enums.LoreRoomSubType GetSubType();
   public int GetWidth();

   public int GetHeight();

   public bool IsWallSpawn();
   
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IPlaceable
{
   Enums.LoreRoomSubType SubType
   {
      get;
   }

   Enums.PlacementType PlacementType{
      get;
   }

   int Width
   {
      get;
   }

   int Height{
      get;
   }

   int MaximumNumberAllowed{
      get;
   }

}
